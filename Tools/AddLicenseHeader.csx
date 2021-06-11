/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

const string LINE1 = "/* This Source Code Form is subject to the terms of the Mozilla Public";
const string LINE2 = " * License, v. 2.0. If a copy of the MPL was not distributed with this";
const string LINE3 = " * file, You can obtain one at https://mozilla.org/MPL/2.0/. */";

if (Args.Count != 1) {
    WriteLine("対象のディレクトリを指定してください。");
    return;
}

string   dir   = Path.GetFullPath(Args[0]);
string[] files = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);
int      added = 0;

WriteLine("対象のディレクトリ：" + dir);
WriteLine();

for (int i = 0; i < files.Length; ++i) {
    string file = files[i];
    WriteLine("\"{0}\" を確認しています . . .", file);
    using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
    using (var sr = new StreamReader(fs, true))
    using (var sw = new StreamWriter(fs, Encoding.UTF8)) {
        if ((sr.ReadLine() is not null and string line1) && line1 == LINE1 &&
            (sr.ReadLine() is not null and string line2) && line2 == LINE2 &&
            (sr.ReadLine() is not null and string line3) && line3 == LINE3) {
            WriteLine("ライセンス通知は記述されています。");
        } else {
            fs.Seek(0, SeekOrigin.Begin);
            sw.WriteLine(LINE1);
            sw.WriteLine(LINE2);
            sw.WriteLine(LINE3);
            sw.WriteLine();
            ++added;
            WriteLine("ライセンス通知を追加しました。");
        }
    }
}

WriteLine();
WriteLine("{0}個中{1}個のファイルにライセンス通知を追加しました。", files.Length, added);
