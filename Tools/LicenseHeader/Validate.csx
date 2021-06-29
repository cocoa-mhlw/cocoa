/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

if (Args.Count != 1) {
	WriteLine("対象のディレクトリを指定してください。");
	return;
}

string   dir           = Path.GetFullPath(Args[0]);
string[] files         = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
var      invalid_files = new List<string>();

WriteLine("対象のディレクトリ：" + dir);
WriteLine();

for (int i = 0; i < files.Length; ++i) {
	string file = files[i];
	string ext  = Path.GetExtension(file);
	WriteLine("\"{0}\" を確認しています . . .", file);
	if (!Ignore(file) && ext is ".cs" or ".csx" or ".strings" or ".xml" or ".xaml" or ".resx"
		or ".axml" or ".storyboard" or ".bat" or ".cmd" or ".sh" or ".tf" or ".yml" or ".feature") {
		using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
		using (var sr = new StreamReader(fs, true)) {
			if (Validate(sr.ReadToEnd())) {
				WriteLine("ライセンス通知は記述されています。");
			} else {
				invalid_files.Add(file);
				ForegroundColor = ConsoleColor.Red;
				WriteLine("ライセンス通知は記述されていません。");
				ResetColor();
			}
		}
	} else {
		WriteLine("このファイルは無視されます。");
	}
	WriteLine();
}

WriteLine();

int count = invalid_files.Count;
if (count == 0) {
	WriteLine("ライセンス通知は全てのファイルに記述されています。");
} else {
	var sb = new StringBuilder();
	sb.AppendLine("ライセンス通知が記述されていないファイルがあります。");
	for (int i = 0; i < count; ++i) {
		sb.Append(" - ").AppendLine(invalid_files[i]);
	}
	ForegroundColor = ConsoleColor.Red;
	WriteLine(sb.ToString());
	throw new LicenseHeaderException(sb.ToString());
}


/*================================================================================================*/

const string HEADER_LINE_1  = "This Source Code Form is subject to the terms of the Mozilla Public";
const string HEADER_LINE_2  = "License, v. 2.0. If a copy of the MPL was not distributed with this";
const string HEADER_LINE_3  = "file, You can obtain one at http://mozilla.org/MPL/2.0/.";
const string HEADER_LINE_3s = "file, You can obtain one at https://mozilla.org/MPL/2.0/.";

/// <summary>検証</summary>
public static bool Validate(string data)
{
	if ((data.Contains(HEADER_LINE_1) && data.Contains(HEADER_LINE_2)) &&
		(data.Contains(HEADER_LINE_3) || data.Contains(HEADER_LINE_3s))) {
		return true;
	} else {
		return false;
	}
}

/// <summary>除外設定</summary>
public static bool Ignore(string file)
{
	return file.EndsWith(".designer.cs")
		|| file.EndsWith(".Designer.cs")
		|| file.EndsWith(".feature.cs")
		|| file.Contains("Xamarin.ExposureNotification")
		|| file.Contains("bin")
		|| file.Contains("Bin")
		|| file.Contains("obj")
		|| file.Contains("Obj")
		|| file.Contains(".github");
}

[Serializable()]
public sealed class LicenseHeaderException : System.Exception
{
	public LicenseHeaderException(string message)
		: base(message) { }

	protected LicenseHeaderException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext  context)
		: base(info, context) { }
}
