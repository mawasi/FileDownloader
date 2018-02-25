using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileDownloader
{
	/// <summary>
	/// デリゲートを受け取るICommandの実装
	/// </summary>
	public class DelegateCommand : ICommand
	{
		/// <summary>
		/// CanExecuteの結果に変更があったことを通知するイベント
		/// WPFのCommandManagerのRequerySuggestedイベントをラップする形で実装しています
		/// </summary>
		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		Action execute;

		Func<bool>	canExecute;

		/// <summary>
		/// コマンドのExecuteメソッドで実行する処理を指定してDelegateCommandのインスタンスを作成します。
		/// </summary>
		/// <param name="execute"></param>
		public DelegateCommand(Action execute) : this(execute, () => true) {}

		/// <summary>
		/// コマンドのExecuteメソッドで実行する処理とCanExecuteメソッドで実行する処理を指定して
		/// DelegateCommandのインスタンスを作成します。
		/// </summary>
		/// <param name="execute"></param>
		/// <param name="canExecute"></param>
		public DelegateCommand(Action execute, Func<bool> canExecute)
		{

			this.execute = execute ?? throw new ArgumentNullException("execute");
			this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
		}

		/// <summary>
		/// コマンドが実行可能かどうか問い合わせます
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter)
		{
			return this.canExecute();
		}

		/// <summary>
		/// コマンドを実行します
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			this.execute();
		}
	}
}
