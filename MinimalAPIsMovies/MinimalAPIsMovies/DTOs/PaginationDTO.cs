using Azure.Storage.Blobs.Models;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIsMovies.Utilities;

namespace MinimalAPIsMovies.DTOs
{
    public class PaginationDTO 
    {
        public const int pageInitialValue = 1;
        public const int recordsPerPageInitialValue = 10;
        public int Page { get; set; }
        private int recordsPerPage = 10;
        private readonly int recordsPerPageMax = 50;

        public int RecordsPerPage 
        { 
            get
            {
                return recordsPerPage;
            }
            set
            {
                if(value > recordsPerPageMax)
                {
                    recordsPerPage = recordsPerPageMax;
                }
                else
                {
                    recordsPerPage = value;
                }
            }
        }

        public static ValueTask<PaginationDTO> BindAsync(HttpContext context)
        {
            //var page = context.Request.Query[nameof(Page)];
            //var recordPerPage = context.Request.Query[nameof(RecordsPerPage)];

            //var pageInt = page.IsNullOrEmpty() ? pageInitialValue : int.Parse(page.ToString());
            //var recordsPerPageInt = recordPerPage.IsNullOrEmpty() ? recordsPerPageInitialValue : int.Parse(recordPerPage.ToString());

            var page = context.ExtractValueOrDefault(nameof(Page), pageInitialValue);
            var recordPerPage = context.ExtractValueOrDefault(nameof(RecordsPerPage), recordsPerPageInitialValue);

            var response = new PaginationDTO
            {
                Page = page,
                RecordsPerPage = recordPerPage
            };

            return ValueTask.FromResult(response);
        }
    }
}
