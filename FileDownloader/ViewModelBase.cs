using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FileDownloader
{
	/// <summary>
	/// ビューモデル基底
	/// </summary>
	public class ViewModelBase : INotifyPropertyChanged
	{
		/// <summary>
		/// プロパティの変更があったときに発行される
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// PropertyChangedイベントを発行
		/// </summary>
		/// <param name="propertyName"></param>
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
