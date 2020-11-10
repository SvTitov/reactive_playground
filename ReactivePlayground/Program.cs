using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactivePlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            SequenceWithDelay();

            ObserveAsyncDownloading();

            InvokeInBusy();

            Console.ReadKey(true);
        }

        private static void InvokeInBusy()
        {
            Observable.Start(MakeBusy)
                .Merge(Observable.FromAsync(() => DownloadData("http://google.com"))
                    .Do(s => Console.WriteLine(s.Substring(0, 5)))
                    .Select(_ => Unit.Default))
                .Finally(MakeUnBusy)
                .Subscribe();
        }

        private static void MakeBusy()
        {
            Console.WriteLine("MakeBusy");
        }

        private static void MakeUnBusy()
        {
            Console.WriteLine("MakeUnBusy");
        }

        private static void ObserveAsyncDownloading()
        {
            Observable.FromAsync(() => DownloadData("http://google.com"))
                .Subscribe(onNext: Console.WriteLine);
        }

        private static void SequenceWithDelay()
        {
            var observer = Observer.Create<string>(Console.WriteLine);
            Observable.Range(0, 5)
                .Zip(Observable.Interval(TimeSpan.FromSeconds(1)), (i, l) => i.ToString())
                .Do(observer)
                .Subscribe();
        }

        private static async Task<string> DownloadData(string url)
        {
            var client = new HttpClient {BaseAddress = new Uri(url)};
            var data = await client.GetAsync(string.Empty);
            return await data.Content.ReadAsStringAsync();
        }
    }
}