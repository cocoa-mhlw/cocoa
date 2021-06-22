/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

if (Args.Count != 1) {
	WriteLine("対象のディレクトリを指定してください。");
	return;
}

var      enc   = new UTF8Encoding(false);
string   dir   = Path.GetFullPath(Args[0]);
string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
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
			var openWriter = new OpenWriter<StreamWriter, (FileStream fs, Encoding enc)>(state => {
				state.fs.SetLength(0);
				return new(state.fs, state.enc) {
					NewLine = "\n"
				};
			});

			string data = sr.ReadToEnd().Replace("\r\n", "\n");
			string name = Path.GetFileNameWithoutExtension(file);
			string ext  = Path.GetExtension(file);

			var result = AddHeader(name, data, ext switch {
				// 拡張子毎にライセンス通知の書式を設定する。
				".cs"         => CreateCStyleHeader,
				".csx"        => CreateCStyleHeader,
				".strings"    => CreateCStyleHeader,
				".xml"        => CreateXMLStyleHeader,
				".xaml"       => CreateXMLStyleHeader,
				".resx"       => CreateXMLStyleHeaderWithAlign,
				".axml"       => CreateXMLStyleHeaderWithAlign,
				".storyboard" => CreateXMLStyleHeaderWithAlign,
				".bat"        => CreateBatchFileStyleHeader,
				".cmd"        => CreateBatchFileStyleHeader,
				".sh"         => CreateShellScriptStyleHeaderWithShebang,
				".tf"         => CreateShellScriptStyleHeader,
				".yml"        => CreateShellScriptStyleHeader,
				".feature"    => CreateShellScriptStyleHeader,

				// それ以外のファイルは書き換えない。
				_ => (filename, line1, line2, line3) => null
			}, openWriter, (fs, enc));

			switch (result) {
			case FileState.NotDefined:
				WriteLine("このファイルに対するライセンス通知は定義されていません。");
				break;
			case FileState.AlreadyAdded:
				WriteLine("ライセンス通知は記述されています。");
				break;
			case FileState.NewlyAdded:
				WriteLine("ライセンス通知を追加しました。");
				++added;
				break;
			default:
				WriteLine("実行結果：{0}", result);
				break;
			}
		}
	}
	WriteLine();
}

WriteLine();
WriteLine("{0}個中{1}個のファイルにライセンス通知を追加しました。", files.Length, added);

/*================================================================================================*/

public const string XML_HEADER_1   = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
public const string XML_HEADER_2   = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
public const string SH_HEADER      = "#!/bin/bash";
public const string FEATURE_HEADER = "Feature: ";

const string HEADER_LINE_1 = "This Source Code Form is subject to the terms of the Mozilla Public";
const string HEADER_LINE_2 = "License, v. 2.0. If a copy of the MPL was not distributed with this";
const string HEADER_LINE_3 = "file, You can obtain one at https://mozilla.org/MPL/2.0/.";

public delegate string? CreateHeader(string filename, string line1, string line2, string line3);

public delegate TWriter OpenWriter<out TWriter, in TState>(TState state)
	where TWriter: TextWriter;

public enum FileState
{
	NotDefined,
	AlreadyAdded,
	NewlyAdded
}

public static FileState AddHeader<TWriter, TState>(
	string                      filename,
	string                      data,
	CreateHeader                createHeader,
	OpenWriter<TWriter, TState> openWriter,
	TState                      argForWriter)
	where TWriter: TextWriter
{
	string? header = createHeader(filename, HEADER_LINE_1, HEADER_LINE_2, HEADER_LINE_3);

	if (header is null) {
		return FileState.NotDefined;
	} else if (data.StartsWith(header)) {
		return FileState.AlreadyAdded;
	} else {
		using (var tw = openWriter(argForWriter)) {
			tw.WriteLine(header);
			tw.Write(data);
		}
		return FileState.NewlyAdded;
	}
}

/// <summary>C言語のブロックコメントと同じ書式で記述できる言語用のライセンス通知を作成する</summary>
public static string CreateCStyleHeader(string filename, string line1, string line2, string line3)
{
	return "/* " + line1 + "\n * " + line2 + "\n * " + line3 + " */\n";
}

/// <summary>XMLのコメントと同じ書式で記述できる言語用のライセンス通知を作成する</summary>
public static string CreateXMLStyleHeader(string filename, string line1, string line2, string line3)
{
	return XML_HEADER_1 + "\n<!-- " + line1 + "\n   - " + line2 + "\n   - " + line3 + " -->\n";
}

/// <summary>XMLのコメントと同じ書式で記述できる言語用のライセンス通知を作成する</summary>
/// <remarks>XML宣言とライセンス通知の間に空行を挿入する</remarks>
public static string CreateXMLStyleHeaderWithAlign(string filename, string line1, string line2, string line3)
{
	return XML_HEADER_2 + "\n\n\n<!-- " + line1 + "\n   - " + line2 + "\n   - " + line3 + " -->\n";
}

/// <summary>バッチファイル用のライセンス通知を作成する</summary>
public static string CreateBatchFileStyleHeader(string filename, string line1, string line2, string line3)
{
	return "@REM " + line1 + "\n@REM " + line2 + "\n@REM " + line3 + '\n';
}

/// <summary>シェルスクリプトのコメントと同じ書式で記述できる言語用のライセンス通知を作成する</summary>
public static string CreateShellScriptStyleHeader(string filename, string line1, string line2, string line3)
{
	return "# " + line1 + "\n# " + line2 + "\n# " + line3 + '\n';
}

/// <summary>シェルスクリプト用のライセンス通知を作成する</summary>
/// <remarks><c>Shebang</c> を挿入する</remarks>
public static string CreateShellScriptStyleHeaderWithShebang(string filename, string line1, string line2, string line3)
{
	return SH_HEADER + '\n' + CreateShellScriptStyleHeader(filename, line1, line2, line3);
}

/// <summary><c>.feature</c>ファイル用のライセンス通知を作成する</summary>
public static string CreateFeatureFileHeader(string filename, string line1, string line2, string line3)
{
	return FEATURE_HEADER + filename + "\n\n" + CreateShellScriptStyleHeader(filename, line1, line2, line3);
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
