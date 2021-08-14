using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Net.Http;
using System.IO;

using AngleSharp.Html;
using AngleSharp.Html.Dom;


/*

http://www.gdcvault.com/gdmag
↑ここのPDFファイルを全部ダウンロードするためのツール！！

ダウンロード中のプログレス出してるプロジェクト
https://github.com/yuxxxx/ProgressiveDownload


mvvm
https://code.msdn.microsoft.com/windowsdesktop/MVVM-d8261534

https://msdn.microsoft.com/magazine/dn605875
https://msdn.microsoft.com/ja-jp/magazine/dn630647.aspx
https://www.kekyo.net/2014/12/21/4638

C# event
http://ufcpp.net/study/csharp/MiscEventSubscribe.html
http://ufcpp.net/study/csharp/sp_event.html

非同期の例外処理
https://www.kekyo.net/2015/06/22/5119


*/


namespace FileDownloader
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
//			URLTextBox.Text = "http://www.gdcvault.com/gdmag";
		}

	}



}
