using KindleConverter_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Utility.Progress;

namespace KindleConverter_WPF
{
    public class Dataflow
    {
        private IEnumerable<Book> books;
        private BookTransfer transfer;
        private Action callback;
        private TransformBlock<Book, Book> beforeConvertBlock;
        private TransformBlock<Book, Book> afterConvertBlock;
        private TransformBlock<Book, Book> convertBlock;
        private TransformBlock<Book, Book> sendConvertBlock;
        private TransformBlock<Book, Book> sendAfterConvertBlock;
        private ActionBlock<Book> callbackBlock;
        private DataflowBlockOptions option;

        public Dataflow(IEnumerable<Book> books,
            BookTransfer transfer,
            Action completed)
        {
            this.books = books;
            this.transfer = transfer;
            callback = completed;
            option = new DataflowBlockOptions();
            option.CancellationToken = transfer.Token.Token;

            ConfigPipeline();
        }

        public Task Execute()
        {
            foreach (var book in books)
            {
                beforeConvertBlock.Post(book);
            }

            beforeConvertBlock.Complete();
            return callbackBlock.Completion;
        }

        private void ConfigPipeline()
        {
            beforeConvertBlock = GetBeforeConvertBlock();
            afterConvertBlock = GetAfterConvertBlock();
            convertBlock = GetConvertBlock();
            sendConvertBlock = GetSendBlock();
            sendAfterConvertBlock = GetAfterSendBlock();
            callbackBlock = GetCompletedBlock();

            beforeConvertBlock.LinkTo(convertBlock);
            convertBlock.LinkTo(afterConvertBlock);
            afterConvertBlock.LinkTo(sendConvertBlock);
            sendConvertBlock.LinkTo(sendAfterConvertBlock);
            sendAfterConvertBlock.LinkTo(callbackBlock);

            beforeConvertBlock.Completion.ContinueWith(t => convertBlock.Complete());
            convertBlock.Completion.ContinueWith(t => afterConvertBlock.Complete());
            afterConvertBlock.Completion.ContinueWith(t => sendConvertBlock.Complete());
            sendConvertBlock.Completion.ContinueWith(t => sendAfterConvertBlock.Complete());  
        }

        private TransformBlock<Book, Book> GetBeforeConvertBlock()
        {
            return new TransformBlock<Book, Book>(book =>
            {
                transfer.BeforeConvert(book);
                return book;
            });
        }

        private TransformBlock<Book, Book> GetAfterConvertBlock()
        {
            return new TransformBlock<Book, Book>(book =>
            {
                transfer.AfterConvert(book);
                return book;
            });
        }

        private TransformBlock<Book, Book> GetConvertBlock()
        {
            return new TransformBlock<Book, Book>(book =>
            {
                transfer.Convert(book);
                return book;
            });
        }

        private TransformBlock<Book, Book> GetSendBlock()
        {
            return new TransformBlock<Book, Book>(book =>
            {
                transfer.Send(book);
                return book;
            });
        }

        private TransformBlock<Book, Book> GetAfterSendBlock()
        {
            return new TransformBlock<Book, Book>(book =>
            {
                transfer.AfterSend(book);
                return book;
            });
        }

        private ActionBlock<Book> GetCompletedBlock()
        {
            return new ActionBlock<Book>(book =>
            {
                callback();
            });
        }
    }
}
