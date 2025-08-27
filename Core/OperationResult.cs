using System.Text.Json.Serialization;

namespace Browser.Core
{
    public class OperationResult
    {
        public bool IsSuccess { get; set; } = true;

        public bool IsBadRequest { get; set; } = false;

        public bool IsException { get; set; } = false;

        private readonly List<string> _messages = [];
        public IEnumerable<string> Messages
        {
            get { return [.. _messages]; }
            set
            {
                _messages.Clear();

                if (value is not null)
                    AddMessages(value);
            }
        }

        /// <summary>
        /// Sets the result to indicate a bad request.
        /// </summary>
        public void SetBadRequest()
        {
            IsSuccess = false;
            IsBadRequest = true;
        }

        /// <summary>
        /// Sets the result to indicate a bad request and adds a message.
        /// </summary>
        /// <param name="message"></param>
        public void SetBadRequest(string message)
        {
            SetBadRequest();
            AddMessage(message);
        }

        /// <summary>
        /// Sets the result to indicate an exception occurred.
        /// </summary>
        public void SetException()
        {
            IsSuccess = false;
            IsException = true;
        }

        /// <summary>
        /// Sets the result to indicate an exception occurred and adds a message.
        /// </summary>
        /// <param name="message"></param>
        public void SetException(string message)
        {
            SetException();
            AddMessage(message);
        }

        /// <summary>
        /// Adds a message to the result if it is not null, empty, 
        /// or whitespace and does not already exist in the list.
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            if (String.IsNullOrWhiteSpace(message))
                return;

            if (_messages.Contains(message))
                return;

            _messages.Add(message);
        }

        /// <summary>
        /// Adds a list of messages to the result. Removes any null, 
        /// empty, whitespace, or duplicate messages.
        /// </summary>
        /// <param name="messages"></param>
        public void AddMessages(IEnumerable<string> messages)
        {
            if (messages is null)
                return;

            foreach (var message in messages)
                AddMessage(message);
        }

        /// <summary>
        /// Removes a specific message from the result.
        /// </summary>
        /// <param name="message"></param>
        public void RemoveMessage(string message) => _messages.Remove(message);

        /// <summary>
        /// Clears all messages from the result;
        /// </summary>
        public void ClearMessages() => _messages.Clear();
    }
}
