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

using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;


/*

http://www.gdcvault.com/gdmag
↑ここのPDFファイルを全部ダウンロードするためのツール！！

ダウンロード中のプログレス出してるプロジェクト
https://github.com/yuxxxx/ProgressiveDownload



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
			URLTextBox.Text = "http://www.gdcvault.com/gdmag";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void DownloadButtonClick(object sender, RoutedEventArgs e)
		{
			// ログクリア
			OutputLog.Text = string.Empty;

			// ダウンロードしたいページのURLが書かれてない場合、そこで終了
			if(string.IsNullOrEmpty(URLTextBox.Text)){
				OutputLog.Text = $"URLが設定されていません。\n";
				return;
			}

			if(string.IsNullOrEmpty(SavePathTextBox.Text)){
				OutputLog.Text = $"SaveDirectoryが設定されていません。\n";
				return;
			}


			OutputLog.Text += $"Download Commences.\n";

			DLButton.IsEnabled = false;

			string url = URLTextBox.Text;
			string savepath = SavePathTextBox.Text;
			await Task.Run(() => ExecuteAsync(url, savepath));

			DLButton.IsEnabled = true;

			OutputLog.Text += $"Download Complete!!\n";

		}


		/// <summary>
		/// 指定のURLからHTMLドキュメント取得
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private async Task<IHtmlDocument> GetHtmlDocumentAsync(string url)
		{
			var doc = default(IHtmlDocument);
			using(var client = new HttpClient()){

				// タイムアウト時間の設定
				client.Timeout = TimeSpan.FromSeconds(10.0);

				try{
					using(var webstream = await client.GetStreamAsync(new Uri(url))){
						var parser = new HtmlParser();
						doc = await parser.ParseAsync(webstream);
					}
				}
				catch(Exception e){
					throw;
				}

			}

			return doc;
		}


		/// <summary>
		/// URLを指定してダウンロード
		/// </summary>
		/// <param name="url"></param>
		/// <param name="savepath"></param>
		/// <returns></returns>
		async Task<bool> Download(string url, string savepath)
		{
			try{
				string filename = System.IO.Path.GetFileName(url);
				string save = $"{savepath}\\{filename}";

				Dispatcher.Invoke(() => OutputLog.Text += $"{url}");

				using(var client = new HttpClient()){
					// ウィンドウにプログレスバーを追加したいが、進捗見るならここでやる必要がある。どうやったらいいのか？
					using(var data = await client.GetStreamAsync(new Uri(url))){
						// このタイミングではまだDL完了してない
	//							OutputLog.Text += $"DL完了\n";
						using(var fs = new System.IO.FileStream(save, System.IO.FileMode.Create)){
							using(var bw = new System.IO.BinaryWriter(fs)){
								byte[] binary = new byte[1048576];
								int read = 0;
								while((read = await data.ReadAsync(binary, 0, binary.Length)) > 0){
									bw.Write(binary, 0, read);
								}

							}
						}
					}
				}
				Dispatcher.Invoke(() => OutputLog.Text += $"  Complete.\n");

			}
			catch(Exception e){
				throw;
			}

			return true;
		}


		/// <summary>
		/// ダウンロード処理
		/// </summary>
		/// <returns></returns>
		private async Task<bool> ExecuteAsync(string url, string savepath)
		{

			try{
				using(var doc = await Task.Run(() => GetHtmlDocumentAsync(url))){
					// リンク要素取得？よくわかってない。
					var links = doc.Links;

					// pdfとzipのURLリストに分ける
					var pdfs = links.Select(elem => elem.GetAttribute("href")).Where(elem => elem.Contains(".pdf"));
					var zips = links.Select(elem => elem.GetAttribute("href")).Where(elem => elem.Contains(".zip"));


					// todo
					// 上記リストから5個ずつくらいに小分けにしてParallel.ForEach使ってダウンロード処理作る

					Dispatcher.Invoke(() => OutputLog.Text += $"{pdfs.Count()} PDF Files.\n");
					string save = $"{savepath}\\pdf";
					if(!Directory.Exists(save)){
						Directory.CreateDirectory(save);
					}
					foreach(var pdf in pdfs){
						await Download(pdf, save);
//						break;	// とりあえず１個DLしたらおわる
					}

					Dispatcher.Invoke(() => OutputLog.Text += $"{pdfs.Count()} ZIP Files.\n");
					save = $"{savepath}\\zip";
					if(!Directory.Exists(save)){
						Directory.CreateDirectory(save);
					}
					foreach(var zip in zips){
						await Download(zip, save);
//						break;	// 一旦一つダウンロードしたら終わり
					}
				}
			}
			catch(Exception e){
				Dispatcher.Invoke(() => OutputLog.Text += $"\n例外が発生しました。\n{e}\n処理を中止します。");
			}

			return true;
		}
	}



}
