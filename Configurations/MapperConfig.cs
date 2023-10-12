namespace bookstoreApp.Api.Configurations
{
    public class MapperConfig: Profile
    {
        public MapperConfig()
        {
            #region Autor Mappings
            // Definisci una mappatura bidirezionale tra AuthorCreateDto e Author
            CreateMap<AuthorCreateDto, Author>().ReverseMap();

            // Definisci una mappatura bidirezionale tra AuthorUpdateDto e Author
            CreateMap<AuthorUpdateDto, Author>().ReverseMap();

            // Definisci una mappatura bidirezionale tra AuthorReadOnlyDto e Author
            CreateMap<AuthorReadOnlyDto, Author>().ReverseMap();
            #endregion


            #region Book Mappings
            CreateMap<BookCreateDto, Book>().ReverseMap();
            CreateMap<BookUpdateDto, Book>().ReverseMap();

            // Definisci una mappatura tra la classe Book e BookReadOnlyDto
            CreateMap<Book, BookReadOnlyDto>()
                // Personalizza la mappatura del membro AuthorName
                .ForMember(nome => nome.AuthorName, x => x.MapFrom(map => $"{map.Author.FirstName} {map.Author.LastName}"))
                // Abilita la mappatura inversa per semplificare la conversione
                .ReverseMap();


            // Definisci una mappatura bidirezionale tra Book e BookDetailsDto
            CreateMap<Book, BookDetailsDto>()
                // Personalizza la mappatura del membro AuthorName
                .ForMember(nome => nome.AuthorName, x => x.MapFrom(map => $"{map.Author.FirstName} {map.Author.LastName}"))
                // Abilita la mappatura inversa per semplificare la conversione
                .ReverseMap();

            #endregion

        }
    }
}
