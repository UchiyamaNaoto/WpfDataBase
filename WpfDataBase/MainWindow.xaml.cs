using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
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

namespace WpfDataBase {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {

        BitmapImage notPicBmp = null;
        DataRowView drv; //選択しているレコードを格納

        infosys202000DataSet infosys202000DataSet;
        infosys202000DataSetTableAdapters.CarReportTableAdapter infosys202000DataSetCarReportTableAdapter;
        CollectionViewSource carReportViewSource;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            notPicBmp = path2Image("notPic.jpg");
            infosys202000DataSet = ((infosys202000DataSet)(this.FindResource("infosys202000DataSet")));
            // テーブル CarReport にデータを読み込みます。必要に応じてこのコードを変更できます。
            infosys202000DataSetCarReportTableAdapter = new infosys202000DataSetTableAdapters.CarReportTableAdapter();
            infosys202000DataSetCarReportTableAdapter.Fill(infosys202000DataSet.CarReport);
            carReportViewSource = ((CollectionViewSource)(this.FindResource("carReportViewSource")));
            carReportViewSource.View.MoveCurrentToFirst();
        }
        //リストビュー選択
        private void carReportListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //選択行を１レコード読み込み
            drv = (DataRowView)carReportViewSource.View.CurrentItem;
            //上記の読み込んだデータをそれぞれのコントロールへ設定
            idTextBox.Text = drv.Row[0].ToString();
            createdDateDatePicker.SelectedDate = (DateTime)drv.Row[1];
            authorTextBox.Text = drv.Row[2].ToString();
            makerTextBox.Text = drv.Row[3].ToString();
            nameTextBox.Text = drv.Row[4].ToString();
            reportTextBox.Text = drv.Row[5].ToString();
            try {
                //画像がない場合は例外で処理する
                pictureImage.Source = Byte2Image((byte[])drv.Row[6]);
            }
            catch (Exception) {
                pictureImage.Source = notPicBmp;
            }
        }

        //開くボタン
        private void btOpen_Click(object sender, RoutedEventArgs e) {
            // ファイルを開くダイアログ
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEG|*.jpg|PNG|*.png|BMP|*.bmp";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true) {
                // Imageコントロールに表示
                pictureImage.Source = path2Image(dlg.FileName);
            }
        }
        //ファイルパスからイメージへ変換
        private static BitmapImage path2Image(string filename) {
            // BitmapImageにファイルから画像を読み込む
            BitmapImage m_bitmap = new BitmapImage();
            m_bitmap.BeginInit();
            m_bitmap.UriSource = new Uri(filename,UriKind.Relative);
            m_bitmap.EndInit();
            return m_bitmap;
        }

        //バイト配列をイメージへ変換
        private static BitmapImage Byte2Image(byte[] imageData) {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData)) {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        //イメージをバイト配列へ変換
        private static byte[] Image2Byte(BitmapImage m_bitmap) {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(BitmapImage));
            byte[] data = null;
            try {
                if (m_bitmap != null) {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(m_bitmap));   //画像データ
                    using (MemoryStream ms = new MemoryStream()) {
                        encoder.Save(ms);   //エンコード
                        data = ms.ToArray();
                    }
                }
            }
            catch (Exception) { }

            return data;
        }
        //更新ボタン
        private void btUpdate_Click(object sender, RoutedEventArgs e) {
            //drv.Row[0] = idTextBox.Text;  //主キーは変更しない
            drv.Row[1] = createdDateDatePicker.SelectedDate;
            drv.Row[2] = authorTextBox.Text;
            drv.Row[3] = makerTextBox.Text;
            drv.Row[4] = nameTextBox.Text;
            drv.Row[5] = reportTextBox.Text;
            drv.Row[6] = Image2Byte((BitmapImage)pictureImage.Source);
                
            infosys202000DataSetCarReportTableAdapter.Update(infosys202000DataSet.CarReport);

        }

        private void btClear_Click(object sender, RoutedEventArgs e) {

        }
    }
}
