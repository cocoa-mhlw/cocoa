/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

const string HEADER_LF   = "/* This Source Code Form is subject to the terms of the Mozilla Public\n * License, v. 2.0. If a copy of the MPL was not distributed with this\n * file, You can obtain one at https://mozilla.org/MPL/2.0/. */\n";
const string HEADER_CRLF = "/* This Source Code Form is subject to the terms of the Mozilla Public\r\n * License, v. 2.0. If a copy of the MPL was not distributed with this\r\n * file, You can obtain one at https://mozilla.org/MPL/2.0/. */\r\n";

if (Args.Count != 1) {
    WriteLine("対象のディレクトリを指定してください。");
    return;
}

var      enc   = new UTF8Encoding(false);
string   dir   = Path.GetFullPath(Args[0]);
string[] files = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);
int      added = 0;

WriteLine("対象のディレクトリ：" + dir);
WriteLine();

for (int i = 0; i < files.Length; ++i) {
    string file = files[i];
    WriteLine("\"{0}\" を確認しています . . .", file);
    if (Ignore(file)) {
        WriteLine("このファイルは無視されます。");
    } else {
        using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        using (var sr = new StreamReader(fs, true)) {
            string data = sr.ReadToEnd();
            if (data.StartsWith(HEADER_LF) || data.StartsWith(HEADER_CRLF)) {
                WriteLine("ライセンス通知は記述されています。");
            } else {
                fs.SetLength(0);
                using (var sw = new StreamWriter(fs, enc)) {
                    sw.NewLine = "\n";
                    sw.WriteLine(HEADER_LF);
                    sw.Write(data);
                }
                ++added;
                WriteLine("ライセンス通知を追加しました。");
            }
        }
    }
    WriteLine();
}

WriteLine();
WriteLine("{0}個中{1}個のファイルにライセンス通知を追加しました。", files.Length, added);

static bool Ignore(string file)
{
    return file.EndsWith(".designer.cs")
        || file.EndsWith(".Designer.cs")
        || file.EndsWith(".feature.cs")
        || file.Contains("Xamarin.ExposureNotification");
}
