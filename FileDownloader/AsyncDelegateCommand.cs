using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileDownloader
{
	public class AsyncDelegateCommand : ICommand
	{

		/// <summary>
		/// CanExecuteの結果に変更があったことを通知するイベント
		/// WPFのCommandManagerのRequerySuggestedイベントをラップする形で実装しています
		/// </summary>
		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		/// <summary>
		/// 非同期実行
		/// </summary>
		private Func<Task>	executeAsync;
		/// <summary>
		/// 実行可能かどうか
		/// </summary>
		private Func<bool>	canExecute;


		#region method

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="executeAsync">実行したい非同期処理</param>
		public AsyncDelegateCommand(Func<Task> executeAsync) : this(executeAsync, () => true){}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="executeAsync">実行したい非同期処理</param>
		/// <param name="canExecute">実行可能化どうか問い合わせ処理</param>
		public AsyncDelegateCommand(Func<Task> executeAsync, Func<bool> canExecute)
		{

			this.executeAsync = executeAsync ?? throw new ArgumentNullException("executeAsync");
			this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
		}


		/// <summary>
		/// 実行可能かどうか問い合わせます
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter)
		{
			return this.canExecute();
		}

		/// <summary>
		/// 非同期コマンドを実行します
		/// </summary>
		public async void Execute(object parameter)
		{
			await this.executeAsync();
		}

		#endregion

	}
}
