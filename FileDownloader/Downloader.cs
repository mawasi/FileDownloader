using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;

namespace FileDownloader
{
	/// <summary>
	/// ダウンロード処理
	/// モデル部分
	/// </summary>
	public class Downloader
	{

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

#if false	// チェック用にログ出力するだけ
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
#else
			//		foreach(var pdf in pdfs) {
			//			Dispatcher.Invoke(() => OutputLog.Text += $"{pdf}\n");
			//		}
#endif

				}
			}
			catch(Exception /*e*/){
		//		Dispatcher.Invoke(() => OutputLog.Text += $"\n例外が発生しました。\n{e}\n処理を中止します。");
			}

			return true;
		}

		/// <summary>
		/// 指定のURLからHTMLドキュメント取得
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public async Task<IHtmlDocument> GetHtmlDocumentAsync(string url)
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
				catch(Exception){
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
		public async Task<bool> Download(string url, string savepath)
		{
			try{
				string filename = System.IO.Path.GetFileName(url);
				string save = $"{savepath}\\{filename}";

				using(var client = new HttpClient()){
					// todo: ウィンドウにプログレスバーを追加したい
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

			}
			catch(Exception){
				throw;
			}

			return true;
		}
	}
}
