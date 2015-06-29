using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace Ronin.ML.Util
{
    public class AsyncWebText : IDisposable
    {
        readonly HttpClient _client;
        readonly WebTextExtractor _extractor;

        public AsyncWebText()
        {
            _client = new HttpClient();
            _extractor = new WebTextExtractor();
        }

        public async Task<string> Get(Uri path)
        {
            return await Get(path, _extractor.Extract);
        }

        public async Task<string> Get(Uri path, Func<Stream, string> parser)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            try
            {
                using (HttpResponseMessage res = await _client.GetAsync(path))
                {
                    return parser(await res.Content.ReadAsStreamAsync());
                }
            }
            catch (HttpRequestException rex)
            {
                return rex.Message;
            }
        }

        public void Dispose()
        {
            if(_client != null)
                _client.Dispose();
        }
    }
}
