using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MenuAnalyzer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FullImage : ContentPage
	{
		public FullImage (ImageSource passedImage)
		{
			InitializeComponent ();
            
            loadedImage.Source = passedImage;

            
		}
	}
}