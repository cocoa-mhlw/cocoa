﻿/* This Source Code Form is subject to the terms of the Mozilla Public
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
        ///  ログを出力します。
        /// </summary>
        /// <param name="message">出力するメッセージです。</param>
        /// <param name="method">実行中の関数の名前です。</param>
        /// <param name="filePath">実行中の関数を格納しているソースファイルの名前です。</param>
        /// <param name="lineNumber">実行中の関数のソースファイル内での行番号です。</param>
        /// <param name="logLevel">ログレベルです。</param>
        public void Write(string? message, string method, string filePath, int lineNumber, LogLevel logLevel);
    }

    /// <summary>
    ///  <see cref="Covid19Radar.Services.Logs.ILogWriter"/>の実装です。
    /// </summary>
    public sealed class LogWriter : ILogWriter
    {
        [ThreadStatic()]
        private static StringBuilder? _sb;
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
        public void Write(string? message, string method, string filePath, int lineNumber, LogLevel logLevel)
        {
#if !DEBUG
            if (logLevel == LogLevel.Verbose || logLevel == LogLevel.Debug) {
                return;
            }
#endif
            try {
                var    jstNow = Utils.JstNow();
                string row    = CreateLogContentRow(message ?? string.Empty, method, filePath, lineNumber, logLevel, jstNow, _essentials);
                Debug.WriteLine(row);
                this.WriteLine(jstNow, row);
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
        }

        private void WriteLine(DateTime jstNow, string line)
        {
            var    file = _log_file;
            string path = _log_path.LogFilePath(jstNow);
            if (file is null || file.Path != path) {
                var newFile = new LogFile(path, _encoding);
                do {
                    if (Interlocked.CompareExchange(ref _log_file, newFile, file) == file) {
                        newFile.WriteLine(HEADER);
                        file?.Dispose();
                        file = newFile;
                        break;
                    }
                    Thread.Yield();
                    file = _log_file;
                } while (file is null || file.Path != path);
            }
            file.WriteLine(line);
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
            _ = _sb is null ? _sb = new StringBuilder()
                            : _sb.Clear();

            foreach (string col in cols) {
                _sb.Append(",\"");
                foreach (char ch in col) {
                    string? escaped = ch switch {
                        '\t' => "\\t",  '\v' => "\\v",
                        '\r' => "\\r",  '\n' => "\\n",
                        '\\' => "\\\\", '\"' => "\"\"",
                        _ => null
                    };
                    _ = escaped is null ? _sb.Append(ch)
                                        : _sb.Append(escaped);
                }
                _sb.Append('\"');
            }
            _sb.Remove(0, 1);
            return _sb.ToString();
        }

        private sealed class LogFile : IDisposable
        {
            private readonly Encoding           _encoding;
            private readonly Lazy<StreamWriter> _writer;

            internal string Path { get; }

            internal LogFile(string path, Encoding enc)
            {
                string dir = System.IO.Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                this.Path = path;
                _encoding = enc;
                _writer   = new Lazy<StreamWriter>(this.OpenFile, LazyThreadSafetyMode.PublicationOnly);
            }

            private StreamWriter OpenFile()
            {
                return new StreamWriter(new FileStream(this.Path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), _encoding) {
                    AutoFlush = true
                };
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
