using System.Windows.Threading;
using ProductManager.FileUploader;
using System;
using System.Windows.Browser;
using System.Net;
using System.IO;

namespace ProductManager.FileUploader
{
    public class HttpFileUploader : IFileUploader
    {
        private UserFile _file;
        private long _dataLength;
        private long _dataSent;
        private string _initParams;
        private string _uploadUrl;
        private bool _isCanceled = false;

        private Dispatcher _uiDispatcher { get; set; }

        public HttpFileUploader(UserFile file, Dispatcher uiDispatcher)
        {
            _file = file;
            _uiDispatcher = uiDispatcher;
            _dataLength = _file.FileStream.Length;
            _dataSent = 0;

            string httpHandlerName = "HttpUploadFilesHandler.ashx";
            if (string.IsNullOrEmpty(httpHandlerName))
            {
                httpHandlerName = "HttpUploadFilesHandler.ashx";
            }

            // _uploadUrl = http://localhost:25258/HttpUploadFilesHandler.ashx
            _uploadUrl = new CustomUri(httpHandlerName).ToString();
        }


        public void StartUpload(string initParams)
        {
            _initParams = initParams;
            StartUpload();
        }

        public void CancelUpload()
        {
            _isCanceled = true;
        }

        private void StartUpload()
        {
            UriBuilder httpHandlerUriBuilder = new UriBuilder(_uploadUrl);
            httpHandlerUriBuilder.Query = string.Format("&file={0}&path={1}", HttpUtility.UrlDecode(_file.FileName), _file.FolderName);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(httpHandlerUriBuilder.Uri);
            webRequest.Method = "POST";
            webRequest.BeginGetRequestStream(new AsyncCallback(WriteToStreamCallback), webRequest);
        }

        private void WriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            if (_file.FileStream == null || _isCanceled)
                return;

            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream requestStream = webRequest.EndGetRequestStream(asynchronousResult);

            Byte[] buffer = new Byte[4096];
            int bytesRead = 0, tempTotal = 0;

            _file.FileStream.Position = _dataSent;

            long chunkSize = 4096 * 1024 * 40;
            while ((bytesRead = _file.FileStream.Read(buffer, 0, buffer.Length)) != 0
                && tempTotal + bytesRead <= chunkSize
                && _file.State != FileStates.Deleted
                && _file.State != FileStates.Error
                &&  !_isCanceled
                )
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();

                _dataSent += bytesRead;
                tempTotal += bytesRead;

                _uiDispatcher.BeginInvoke(delegate()
                {
                    OnUploadProgressChanged();
                });
            }

            requestStream.Close();
            webRequest.BeginGetResponse(new AsyncCallback(ReadHttpResponseCallback), webRequest);

        }

        private void ReadHttpResponseCallback(IAsyncResult asynchronousResult) 
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());

                string responseString = reader.ReadToEnd();
                reader.Close();

                _uiDispatcher.BeginInvoke(delegate()
                {
                    OnUploadFinishedProgressChanged();
                });

                if (_dataSent < _dataLength)
                {
                    if(_file.State != FileStates.Error
                     && _file.State != FileStates.Deleted
                        )
                    {
                        StartUpload();
                    }
                }
                else
                {
                    _file.FileStream.Close();
                    _file.FileStream.Dispose();

                    _uiDispatcher.BeginInvoke(delegate()
                    {
                        if (UploadFinished != null)
                        {
                            UploadFinished(this, null);
                        }
                    });
                }

            }
            catch (System.Exception ex)
            {
                _file.FileStream.Close();
                _file.FileStream.Dispose();

                _uiDispatcher.BeginInvoke(delegate()
                {
                    if (_file.State != FileStates.Deleted)
                        _file.State = FileStates.Error;
                });
            }
        }

        private void OnUploadProgressChanged()
        {
            _file.BytesUploaded = _dataSent;
        }

        private void OnUploadFinishedProgressChanged()
        {
            _file.BytesUploadedFinished = _dataSent;
        }

        public event System.EventHandler UploadFinished;
    }
}