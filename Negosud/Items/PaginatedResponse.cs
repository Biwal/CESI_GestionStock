using System.Collections.Generic;

namespace Negosud.Items
{
    public class PaginatedResponse<T>
    {
        public string message;
        public bool didError;
        public string errorMessage;
        public List<T> model = new List<T>();
        public double pageSize = 1;
        public double pageNumber = 1;
        public double itemsCount = 0;
        public double pageCount = 0;
    }
}
