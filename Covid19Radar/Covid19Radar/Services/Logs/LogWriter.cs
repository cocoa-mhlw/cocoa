/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Covid19Radar.Common;

namespace Covid19Radar.Services.Logs
{
    /// <summary>
    ///  ログの書き込みを行う機能を提供します。
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        ///  ログ出力を行います。
        /// </summary>
        /// <param name="message">出力するメッセージを指定します。</param>
        /// <param name="method">実行中の関数の名前を指定します。</param>
        /// <param name="filePath">実行中の関数を格納しているソースファイルの名前を指定します。</param>
        /// <param name="lineNumber">実行中の関数のソースファイル内での行番号を指定します。</param>
        /// <param name="logLevel">ログレベルを指定します。</param>
        public void Write(string? message, string? method, string? filePath, int lineNumber, LogLevel logLevel);
    }

    /// <summary>
    ///  <see cref="Covid19Radar.Services.Logs.ILogWriter"/>の実装です。
    /// </summary>
    public sealed class LogWriter : ILogWriter
    {
        [ThreadStatic()]
        private static StringBuilder? _sb_cache;
        private const string DATETIME_FORMAT = "yyyy/MM/dd HH:mm:ss.fffffff";
        private static readonly string HEADER = CreateLogHeaderRow();
        private readonly ILogPathService _log_path;
        private readonly IEssentialsService _essentials;
        private readonly Encoding _encoding;
        private LogFile? _log_file;

        /// <summary>
        ///  型'<see cref="Covid19Radar.Services.Logs.LogWriter"/>'の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="logPath">ログファイルのパスを提供するサービスを指定します。</param>
        /// <param name="essentials">環境情報を提供するサービスを指定します。</param>
        public LogWriter(ILogPathService logPath, IEssentialsService essentials)
        {
            _log_path   = logPath    ?? throw new ArgumentNullException(nameof(logPath));
            _essentials = essentials ?? throw new ArgumentNullException(nameof(essentials));
            _encoding   = Encoding.UTF8; // 文字コードを変更する必要性が出た場合は、外部から注入できる様にする。
        }

        /// <inheritdoc/>
        public void Write(string? message, string? method, string? sourceFile, int lineNumber, LogLevel logLevel)
        {
#if !DEBUG
            if (logLevel == LogLevel.Verbose || logLevel == LogLevel.Debug) {
                return;
            }
#endif
            try {
                // null は空文字として扱う。
                message    ??= string.Empty;
                method     ??= string.Empty;
                sourceFile ??= string.Empty;

                // ローカル変数初期化。
                var    time = Utils.JstNow();
                var    file = _log_file;
                string path = _log_path.LogFilePath(time);

                // ファイルを更新する必要がある場合。
                if (file is null || file.Path != path) {
                    // 新しいファイルを生成する。
                    var newFile = new LogFile(path, _encoding);

                    do {
                        if (Interlocked.CompareExchange(ref _log_file, newFile, file) == file) {
                            // _log_file を新しいファイルに置き換える事ができた場合、
                            // 元のファイルを破棄しループから抜ける。
                            file?.Dispose();
                            file = newFile;
                            break;
                        }

                        // 別スレッドに制御を渡す。
                        Thread.Yield();

                        // ローカル変数を更新する。
                        time = Utils.JstNow();
                        file = _log_file;
                        path = _log_path.LogFilePath(time);

                        if (newFile.Path != path) {
                            // 日付が更新された場合は新しいファイルを作り直す。
                            newFile.Dispose();
                            newFile = new LogFile(path, _encoding);
                        }
                    } while (file is null || file.Path != path);
                }

                // ログをファイルに書き込む。
                string line = CreateLogContentRow(message, method, sourceFile, lineNumber, logLevel, time, _essentials);
                file .WriteLine(line);
                Debug.WriteLine(line);
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
        }

        private static string CreateLogHeaderRow()
        {
            return CreateLogRow(
                "output_date",
                "log_level",
                "message",
                "method",
                "file_path",
                "line_number",
                "platform",
                "platform_version",
                "model",
                "device_type",
                "app_version",
                "build_number"
            );
        }

        private static string CreateLogContentRow(
            string             message,
            string             method,
            string             filePath,
            int                lineNumber,
            LogLevel           logLevel,
            DateTime           jstDateTime,
            IEssentialsService essentials)
        {
            return CreateLogRow(
                jstDateTime.ToString(DATETIME_FORMAT),
                logLevel.ToString(),
                message,
                method,
                filePath,
                lineNumber.ToString(),
                essentials.Platform,
                essentials.PlatformVersion,
                essentials.Model,
                essentials.DeviceType,
                essentials.AppVersion,
                essentials.BuildNumber
            );
        }

        private static string CreateLogRow(params string[] cols)
        {
            var sb = _sb_cache is null ? _sb_cache = new StringBuilder()
                                       : _sb_cache.Clear();
            foreach (string col in cols) {
                sb.Append(",\"");
                foreach (char ch in col) {
                    string? escaped = ch switch {
                        '\t' => "\\t",  '\v' => "\\v",
                        '\r' => "\\r",  '\n' => "\\n",
                        '\\' => "\\\\", '\"' => "\"\"",
                        _ => null
                    };
                    _ = escaped is null ? sb.Append(ch)
                                        : sb.Append(escaped);
                }
                sb.Append('\"');
            }
            sb.Remove(0, 1);
            return sb.ToString();
        }

        private static FileStream OpenLogFile(string path, bool useWriterMode)
        {
            return new FileStream(
                path,
                useWriterMode ? FileMode.Append  : FileMode.Open,
                useWriterMode ? FileAccess.Write : FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);
        }

        public static StreamReader OpenReader(string path)
        {
            return new StreamReader(OpenLogFile(path, false));
        }

        private sealed class LogFile : IDisposable
        {
            private readonly Encoding _encoding;
            private readonly Lazy<TextWriter> _writer;

            internal string Path { get; }

            internal LogFile(string path, Encoding enc)
            {
                string dir = System.IO.Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                this.Path = path;
                _encoding = enc;
                _writer   = new Lazy<TextWriter>(this.OpenFile, LazyThreadSafetyMode.ExecutionAndPublication);
            }

            private TextWriter OpenFile()
            {
                var fs = OpenLogFile(this.Path, true);
                var sw = TextWriter.Synchronized(new StreamWriter(fs, _encoding) {
                    AutoFlush = true
                });
                if (fs.Length <= _encoding.Preamble.Length) {
                    sw.WriteLine(HEADER);
                }
                return sw;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void WriteLine(string line)
            {
                _writer.Value.WriteLine(line);
            }

            public void Dispose()
            {
                if (_writer.IsValueCreated) {
                    _writer.Value.Dispose();
                }
            }
        }
    }
}
