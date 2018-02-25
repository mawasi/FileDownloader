using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileDownloader
{
	public class MainWindowViewModel : ViewModelBase
	{
		/// <summary>
		/// ダウンロード元のURL
		/// </summary>
		string _URL;
		/// <summary>
		/// セーブディレクトリ
		/// </summary>
		string _SavePath;
		/// <summary>
		/// 実行ログ
		/// </summary>
		string _Log;

		/// <summary>
		/// 
		/// </summary>
		AsyncDelegateCommand	_DownloadCommand;


		public string URL {
			get { return _URL; }
			set {
				_URL = value;
				OnPropertyChanged("URL");
			}
		}

		public string SavePath {
			get { return _SavePath; }
			set {
				_SavePath = value;
				OnPropertyChanged("SavePath");
			}
		}

		public string Log {
			get { return _Log; }
			set {
				_Log = value;
				OnPropertyChanged("Log");
			}
		}

		public AsyncDelegateCommand DownloadCommand{
			get {
				if(_DownloadCommand == null){
					_DownloadCommand = new AsyncDelegateCommand(Execute, CanExecute);
				}
				return _DownloadCommand;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel()
		{
			URL = "http://www.gdcvault.com/gdmag";
		}


		async Task Execute()
		{
			Log = "";

			// ダウンロードしたいページのURLが書かれてない場合、そこで終了
			if(string.IsNullOrEmpty(URL)){
				Log = $"URLが設定されていません。\n";
				return;
			}

			if(string.IsNullOrEmpty(SavePath)){
				Log = $"SaveDirectoryが設定されていません。\n";
				return;
			}


			Log += $"Download Commences.\n";

			string url = URL;
			string savepath = SavePath;
			// ダウンロード処理
			await DownloadAsync(url, savepath);


			Log += $"Download Complete!!\n";
		}

		bool CanExecute()
		{
			return true;
		}


		/// <summary>
		/// ダウンロード処理
		/// </summary>
		/// <param name="url"></param>
		/// <param name="savepath"></param>
		/// <returns></returns>
		async Task<bool> DownloadAsync(string url, string savepath)
		{
			Downloader downloader = new Downloader();

			try{
				using(var doc = await Task.Run(() => downloader.GetHtmlDocumentAsync(url))){
					// リンク要素取得？よくわかってない。
					var links = doc.Links;

					// pdfとzipのURLリストに分ける
					var pdfs = links.Select(elem => elem.GetAttribute("href")).Where(elem => elem.Contains(".pdf"));
					var zips = links.Select(elem => elem.GetAttribute("href")).Where(elem => elem.Contains(".zip"));

#if true
					// todo
					// 上記リストから5個ずつくらいに小分けにしてParallel.ForEach使ってダウンロード処理作る

					Log += $"{pdfs.Count()} PDF Files.\n";
					string save = $"{savepath}\\pdf";
					if(!Directory.Exists(save)){
						Directory.CreateDirectory(save);
					}
					foreach(var pdf in pdfs){
						Log += $"{pdf}";
						await downloader.Download(pdf, save);
						Log += $"  Complete.\n";
						break;	// とりあえず１個DLしたらおわる
					}

					Log += $"{pdfs.Count()} ZIP Files.\n";
					save = $"{savepath}\\zip";
					if(!Directory.Exists(save)){
						Directory.CreateDirectory(save);
					}
					foreach(var zip in zips){
						Log += $"{zip}";
						await downloader.Download(zip, save);
						Log += $"  Complete.\n";
						break;	// 一旦一つダウンロードしたら終わり
					}
#else	// チェック用にログ出力するだけ
					foreach(var pdf in pdfs) {
						Log += $"{pdf}\n";
					}
#endif

				}
			}
			catch(Exception e){
				Log += $"\n例外が発生しました。\n{e}\n処理を中止します。";
			}

			return true;
		}
	}
}
