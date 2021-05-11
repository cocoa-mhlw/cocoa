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
    public interface ILogWriter : IDisposable
    {
        /// <summary>
        ///  ログを出力します。
        /// </summary>
        /// <param name="message">出力するメッセージです。</param>
        /// <param name="method">実行中の関数の名前です。</param>
        /// <param name="filePath">実行中の関数を格納しているソースファイルの名前です。</param>
        /// <param name="lineNumber">実行中の関数のソースファイル内での行番号です。</param>
        /// <param name="logLevel">ログレベルです。</param>
        public void Write(string message, string method, string filePath, int lineNumber, LogLevel logLevel);
    }

    /// <summary>
    ///  <see cref="Covid19Radar.Services.Logs.ILogWriter"/>の実装です。
    /// </summary>
    public sealed class LogWriter : ILogWriter
    {
        private const string DATETIME_FORMAT = "yyyy/MM/dd HH:mm:ss.fffffff";
        private static readonly string HEADER = CreateLogHeaderRow();
        private readonly ILogPathService logPath;
        private readonly IEssentialsService essentials;
        private readonly Encoding enc;
        private File? file;

        /// <summary>
        ///  型'<see cref="Covid19Radar.Services.Logs.LogWriter"/>'の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="logPath">ログファイルのパスを提供するサービスを指定します。</param>
        /// <param name="essentials">環境情報を提供するサービスを指定します。</param>
        public LogWriter(ILogPathService logPath, IEssentialsService essentials)
        {
            this.logPath    = logPath    ?? throw new ArgumentNullException(nameof(logPath));
            this.essentials = essentials ?? throw new ArgumentNullException(nameof(essentials));
            enc = Encoding.UTF8;
        }

        /// <inheritdoc/>
        public void Write(string message, string method, string filePath, int lineNumber, LogLevel logLevel)
        {
#if !DEBUG
            if (logLevel == LogLevel.Verbose || logLevel == LogLevel.Debug) {
                return;
            }
#endif
            try {
                var    jstNow = Utils.JstNow();
                string row    = CreateLogContentRow(message, method, filePath, lineNumber, logLevel, jstNow, essentials);
                Debug.WriteLine(row);
                this.WriteLine(jstNow, row);
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
        }

        private void WriteLine(DateTime jstNow, string line)
        {
            var    file  = this.file;
            string fname = logPath.LogFilePath(jstNow);
            if (file is null || file.FileName != fname) {
                var newFile = new File(fname, enc);
                RewriteField(ref this.file, newFile, ref file);
                file?.Dispose();
                file = newFile;
                file.Writer.WriteLine(HEADER);
            }
            file.Writer.WriteLine(line);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            var file = this.file;
            RewriteField(ref this.file, null, ref file);
            file?.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RewriteField<T>(ref T field, T newValue, ref T oldValue) where T: class?
        {
            while (Interlocked.CompareExchange(ref field, newValue, oldValue) != oldValue) {
                Thread.Yield();
                oldValue = field;
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
            var sb = new StringBuilder();
            for (int i = 0; i < cols.Length; ++i) {
                if (i != 0) {
                    sb.Append(", ");
                }
                string s = cols[i];
                sb.Append('\"');
                for (int j = 0; j < s.Length; ++j) {
                    char ch = s[j];
                    switch (ch) {
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\v':
                        sb.Append("\\v");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\\':
                    case '\"':
                        sb.Append(ch).Append(ch);
                        break;
                    default:
                        sb.Append(ch);
                        break;
                    }
                }
                sb.Append('\"');
            }
            return sb.ToString();
        }

        private sealed class File : IDisposable
        {
            internal string       FileName { get; }
            internal StreamWriter Writer   { get; }

            internal File(string path, Encoding enc)
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                this.FileName = path;
                this.Writer   = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), enc);
                this.Writer.AutoFlush = true;
            }

            public void Dispose()
            {
                this.Writer.Dispose();
            }
        }
    }
}
