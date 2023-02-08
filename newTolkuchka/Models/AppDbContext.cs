using Microsoft.EntityFrameworkCore;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Models
{
    public class AppDbContext : DbContext
    {
        private readonly ICrypto _crypto;
        public AppDbContext(DbContextOptions<AppDbContext> options, ICrypto crypto)
            : base(options)
        {
            _crypto = crypto;
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryAdLink> CategoryAdLinks { get; set; }
        public DbSet<CategoryModelAdLink> CategoryModelAdLinks { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Heading> Headings { get; set; }
        public DbSet<HeadingArticle> HeadingArticles { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<ModelSpec> ModelSpecs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSpecsValue> ProductSpecsValues { get; set; }
        public DbSet<ProductSpecsValueMod> ProductSpecsValueMods { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<Spec> Specs { get; set; }
        public DbSet<SpecsValue> SpecsValues { get; set; }
        public DbSet<SpecsValueMod> SpecsValueMods { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<Wish> Wishes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<Article>().HasIndex(i => i.Name).IsUnique();
            builder.Entity<Heading>().HasIndex(i => new { i.Language, i.Name }).IsUnique();
            builder.Entity<Employee>().HasIndex(i => i.Login).IsUnique();
            builder.Entity<User>().HasIndex(i => i.Email).IsUnique();
            builder.Entity<User>().HasIndex(i => i.Phone).IsUnique();
            builder.Entity<Brand>().HasIndex(i => i.Name).IsUnique();
            builder.Entity<Category>().HasIndex(i => new { i.ParentId, i.NameRu }).IsUnique();
            builder.Entity<Category>().HasIndex(i => new { i.ParentId, i.NameEn }).IsUnique();
            builder.Entity<Category>().HasIndex(i => new { i.ParentId, i.NameTm }).IsUnique();
            builder.Entity<Currency>().HasIndex(i => i.CodeName).IsUnique();
            builder.Entity<Line>().HasIndex(i => new { i.BrandId, i.Name }).IsUnique();
            builder.Entity<Model>().HasIndex(i => new { i.LineId, i.TypeId, i.Name }).IsUnique();
            builder.Entity<Position>().HasIndex(i => i.Name).IsUnique();
            builder.Entity<Spec>().HasIndex(i => i.NameRu).IsUnique();
            builder.Entity<Spec>().HasIndex(i => i.NameEn).IsUnique();
            builder.Entity<Spec>().HasIndex(i => i.NameTm).IsUnique();
            builder.Entity<SpecsValue>().HasIndex(i => new { i.SpecId, i.NameRu }).IsUnique();
            builder.Entity<SpecsValue>().HasIndex(i => new { i.SpecId, i.NameEn }).IsUnique();
            builder.Entity<SpecsValue>().HasIndex(i => new { i.SpecId, i.NameTm }).IsUnique();
            builder.Entity<Type>().HasIndex(i => i.NameRu).IsUnique();
            builder.Entity<Type>().HasIndex(i => i.NameEn).IsUnique();
            builder.Entity<Type>().HasIndex(i => i.NameTm).IsUnique();
            builder.Entity<Warranty>().HasIndex(i => i.NameRu).IsUnique();
            builder.Entity<Warranty>().HasIndex(i => i.NameEn).IsUnique();
            builder.Entity<Warranty>().HasIndex(i => i.NameTm).IsUnique();
            builder.Entity<HeadingArticle>().HasKey(x => new { x.HeadingId, x.ArticleId });
            builder.Entity<CategoryAdLink>().HasKey(x => new { x.CategoryId, x.StepParentId });
            builder.Entity<CategoryModelAdLink>().HasKey(x => new { x.CategoryId, x.ModelId });
            builder.Entity<CategoryModelAdLink>().HasOne(x => x.Model).WithMany(x => x.CategoryModelAdLinks).OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<ModelSpec>().HasKey(x => new { x.ModelId, x.SpecId });
            builder.Entity<ProductSpecsValue>().HasKey(x => new { x.ProductId, x.SpecsValueId });
            builder.Entity<ProductSpecsValueMod>().HasKey(x => new { x.ProductId, x.SpecsValueModId });
            builder.Entity<Wish>().HasKey(x => new { x.UserId, x.ProductId });
            builder.Entity<Position>().HasData(
                new Position
                {
                    Id = 1,
                    Name = "Владелец",
                    Level = 4
                },
                new Position
                {
                    Id = 2,
                    Name = "Программист",
                    Level = 3
                },
                new Position
                {
                    Id = 3,
                    Name = "Менеджер",
                    Level = 2
                },
                new Position
                {
                    Id = 4,
                    Name = "Оператор",
                    Level = 1
                });
            builder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Login = "ayna",
                    Password = _crypto.EncryptString("!Varta"),
                    Hash = "1",
                    PositionId = 1
                },
                new Employee
                {
                    Id = 2,
                    Login = "durdy",
                    Password = _crypto.EncryptString("!Qaz55xsw2"),
                    Hash = "1",
                    PositionId = 2
                });
            builder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Durdy",
                    Phone = "99365811482",
                    Email = "durdysagty@mail.ru",
                    BirthDay = DateTime.Parse("11.05.1984"),
                    Pin = _crypto.EncryptString("1111"),
                    Address = "Magtymguly 150/10"
                });
            builder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = 1,
                    CodeName = "USD",
                    PriceRate = 1M,
                    RealRate = 1M
                },
                new Currency
                {
                    Id = 2,
                    CodeName = "TMT",
                    PriceRate = 20.5M,
                    RealRate = 19.5M
                });
            builder.Entity<Supplier>().HasData(
                new Supplier
                {
                    Id = 1,
                    Name = "Ak yol",
                    PhoneMain = "280401",
                    PhoneSecondary = "726849",
                    Address = "11 мкр-н, ул. А.Ниязова 44"
                },
                new Supplier
                {
                    Id = 2,
                    Name = "Hajy",
                    PhoneMain = "99364919890",
                    Address = "Оптовка"
                });
            builder.Entity<Type>().HasData(
                new Type
                {
                    Id = 1,
                    NameRu = "Мобильный телефон",
                    NameEn = "Mobile phone",
                    NameTm = "Öýjukli telefon"
                },
                new Type
                {
                    Id = 2,
                    NameRu = "Ноутбук",
                    NameEn = "Laptop",
                    NameTm = "Noutbuk"
                },
                new Type
                {
                    Id = 3,
                    NameRu = "Моноблок",
                    NameEn = "Monoblock",
                    NameTm = "Monoblok"
                },
                new Type
                {
                    Id = 4,
                    NameRu = "Холодильник",
                    NameEn = "Refrigerator",
                    NameTm = "Doňduryjy"
                },
                new Type
                {
                    Id = 5,
                    NameRu = "Подставка для ноутбука",
                    NameEn = "Notebook stand",
                    NameTm = "Noutbuk goltugy"
                });
            builder.Entity<Brand>().HasData(
                new Brand
                {
                    Id = 1,
                    Name = "Apple"
                },
                new Brand
                {
                    Id = 2,
                    Name = "Samsung"
                },
                new Brand
                {
                    Id = 3,
                    Name = "DELL"
                },
                new Brand
                {
                    Id = 4,
                    Name = "Adidas"
                }, new Brand
                {
                    Id = 5,
                    Name = "Reebok"
                }, new Brand
                {
                    Id = 6,
                    Name = "Kensington"
                });
            builder.Entity<Line>().HasData(
                new Line
                {
                    Id = 1,
                    Name = "iPhone",
                    BrandId = 1
                },
                new Line
                {
                    Id = 2,
                    Name = "iPad",
                    BrandId = 1
                },
                new Line
                {
                    Id = 3,
                    Name = "Macbook",
                    BrandId = 1
                },
                new Line
                {
                    Id = 4,
                    Name = "Galaxy",
                    BrandId = 2
                },
                new Line
                {
                    Id = 5,
                    Name = "The Frame",
                    BrandId = 2
                },
                new Line
                {
                    Id = 6,
                    Name = "Latitude",
                    BrandId = 3
                },
                new Line
                {
                    Id = 7,
                    Name = "Inspiron",
                    BrandId = 3
                },
                new Line
                {
                    Id = 8,
                    Name = "Yeezy",
                    BrandId = 4
                },
                new Line
                {
                    Id = 9,
                    Name = "Predator",
                    BrandId = 4
                }
                );
            builder.Entity<Model>().HasData(
               new Model
               {
                   Id = 1,
                   Name = "SE 2022",
                   BrandId = 1,
                   LineId = 1,
                   CategoryId = 11,
                   TypeId = 1,
                   WarrantyId = 2,
                   DescRu = "Мощный процессор, элегантный дизайн, расширенный цветовой диапазон экрана, возможность подключения двух пар наушников одновременно, и многое другое — все это вы получите с невероятно производительным Apple iPhone SE 32-го поколения! В нём есть все что вы любите — и даже больше! Apple iPhone SE 3 оснащен самым быстрым процессором A15 Bionic, который выполняет любые поставленные задачи быстро и эффективно! Это настоящая технология будущего, которая совмещает в себе низкую энергозатратность при высокой работоспособности. Apple iPhone SE 3 создан, чтобы стать вашим идеальным смартфоном!",
                   DescEn = "Powerful processor, elegant design, extended screen color range, the ability to connect two pairs of headphones at the same time, and much more - you get it all with the incredible performance of the 32nd generation Apple iPhone SE! It has everything you love - and more! Apple iPhone SE 3 is equipped with the fastest A15 Bionic processor, which performs any tasks quickly and efficiently! This is a real technology of the future, which combines low energy consumption with high performance. Apple iPhone SE 3 is designed to be your ideal smartphone!",
                   DescTm = "Güýçli prosessor, ajaýyp dizaýn, giňeldilen ekran reňk diapazony, bir wagtyň özünde iki jübüt nauşnik birikdirmek ukyby we başga-da köp zat - hemmesini 32-nji nesil Apple iPhone SE-niň ajaýyp öndürijiligi bilen alarsyňyz! Onda siziň söýýän zatlaryňyzyň hemmesi we başgalar bar! Apple iPhone SE 3, islendik meseläni çalt we netijeli ýerine ýetirýän iň çalt A15 Bionic prosessor bilen enjamlaşdyrylandyr! Bu, pes energiýa sarp edilişini ýokary öndürijilik bilen birleşdirýän geljegiň hakyky tehnologiýasy. Apple iPhone SE 3 ideal smartfon siziň üçin döredildi!"
               }, new Model
               {
                   Id = 2,
                   Name = "13 Pro",
                   BrandId = 1,
                   LineId = 1,
                   CategoryId = 11,
                   TypeId = 1,
                   WarrantyId = 2,
                   DescRu = "Дисплей Super Retina XDR с технологией ProMotion и быстрым, плавным откликом. Грандиозный апгрейд системы камер, открывающий совершенно новые возможности. Исключительная прочность. A15 Bionic — самый быстрый чип для iPhone. И впечатляющее время работы без подзарядки. Всё это Pro. Хирургическая нержавеющая сталь, панель Ceramic Shield, надёжная защита от воды (IP68) — всё это невероятно красиво и исключительно прочно. Встречайте дисплей Super Retina XDR с технологией ProMotion. У него адаптивная частота обновления до 120 Гц и великолепная графическая производительность — прикоснитесь и удивитесь.",
                   DescEn = "Super Retina XDR display with ProMotion technology and fast, smooth response. A massive upgrade to the camera system that opens up completely new possibilities. Exceptional strength. A15 Bionic is the fastest iPhone chip. And impressive battery life. Everything is Pro. Surgical stainless steel, Ceramic Shield panel, reliable water protection (IP68) - all this is incredibly beautiful and exceptionally durable. Meet the Super Retina XDR display with ProMotion technology. It has an adaptive refresh rate up to 120Hz and amazing graphics performance - touch and be amazed.",
                   DescTm = "ProMotion tehnologiýasy we çalt, rahat seslenme bilen Super Retina XDR displeýi. Doly täze mümkinçilikleri açýan kamera ulgamyna köpçülikleýin täzeleniş. Adatdan daşary güýç. “A15 Bionic” iň çalt “iPhone” çipidir. Batareýanyň täsirli ömri. Hemme zat Pro. Hirurgiki poslamaýan polat, keramiki galkan paneli, ygtybarly suw goragy (IP68) - bularyň hemmesi ajaýyp owadan we ajaýyp. ProMotion tehnologiýasy bilen Super Retina XDR ekrany bilen tanyşyň. 120Hz-a çenli uýgunlaşdyrylan täzeleniş tizligi we ajaýyp grafiki öndürijiligi bar - degiň we haýran galyň."
               },
               new Model
               {
                   Id = 3,
                   Name = "13 Pro Max",
                   BrandId = 1,
                   LineId = 1,
                   CategoryId = 11,
                   TypeId = 1,
                   WarrantyId = 2,
                   DescRu = "Дисплей Super Retina XDR с технологией ProMotion и быстрым, плавным откликом. Грандиозный апгрейд системы камер, открывающий совершенно новые возможности. Исключительная прочность. A15 Bionic — самый быстрый чип для iPhone. И впечатляющее время работы без подзарядки. Всё это Pro. Хирургическая нержавеющая сталь, панель Ceramic Shield, надёжная защита от воды (IP68) — всё это невероятно красиво и исключительно прочно. В этом апгрейде значительно обновлены и аппаратная часть, и программное обеспечение. Теперь для сверхширокоугольной камеры доступен режим макросъёмки, для телефотокамеры — трёхкратный оптический зум, а ночной режим поддерживается на всех трёх камерах.",
                   DescEn = "Super Retina XDR display with ProMotion technology and fast, smooth response. A massive upgrade to the camera system that opens up completely new possibilities. Exceptional strength. A15 Bionic is the fastest iPhone chip. And impressive battery life. Everything is Pro. Surgical stainless steel, Ceramic Shield panel, reliable water protection (IP68) - all this is incredibly beautiful and exceptionally durable. This upgrade significantly upgrades both the hardware and the software. Macro mode is now available for the ultra-wide camera, 3x optical zoom for the telephoto camera, and night mode is supported on all three cameras.",
                   DescTm = "ProMotion tehnologiýasy we çalt, rahat seslenme bilen Super Retina XDR displeýi.Doly täze mümkinçilikleri açýan kamera ulgamyna köpçülikleýin täzeleniş.Adatdan daşary güýç. “A15 Bionic” iň çalt “iPhone” çipidir.Batareýanyň täsirli ömri.Hemme zat Pro.Hirurgiki poslamaýan polat, keramiki galkan paneli, ygtybarly suw goragy(IP68) - bularyň hemmesi ajaýyp owadan we ajaýyp.Bu täzelenme enjamlary we programma üpjünçiligini ep - esli ýokarlandyrýar.Ultra giň kamera üçin makro re modeimi, telefon kamerasy üçin 3x optiki ýakynlaşdyrma we üç kameranyň hemmesinde gijeki re modeim goldanýar."
               },
            new Model
            {
                Id = 4,
                Name = "13",
                BrandId = 1,
                LineId = 1,
                CategoryId = 11,
                TypeId = 1,
                WarrantyId = 2,
                DescRu = "Дисплей OLED стал на 28% ярче — до 800 кд/ м². На нём всё хорошо видно даже в самый солнечный день. А яркость при просмотре контента в HDR достигает 1200 кд/ м². Вы сможете различить мельчайшие оттенки чёрного и белого — как и всех остальных цветов. При этом дисплей расходует заряд аккумулятора ещё более экономно, чем прежде. Дисплей Super Retina XDR отличается невероятно высокой плотностью пикселей — фотографии, видео и текст выглядят поразительно чётко. А благодаря уменьшенной площади камеры TrueDepth на дисплее теперь больше места для изображения. iPhone 13 работает от аккумулятора до 2,5 часов дольше. Процессор A15 Bionic и камера TrueDepth также обеспечивают работу Face ID, невероятно надёжной технологии аутентификации. Сверхбыстрый чип A15 Bionic обеспечивает работу режима «Киноэффект», фотографических стилей и других функций. Secure Enclave защищает персональные данные, в том числе Face ID и контакты. А ещё новый чип увеличивает время работы от аккумулятора.",
                DescEn = "The OLED display has become 28% brighter - up to 800 cd/ m². Everything is clearly visible on it even on the sunniest day. And the brightness when viewing content in HDR reaches 1200 cd/ m². You will be able to distinguish the smallest shades of black and white - as well as all other colors. At the same time, the display consumes battery power even more economically than before. The Super Retina XDR display features an incredibly high pixel density, making photos, videos and text look amazingly crisp. And thanks to the smaller area of ​​the TrueDepth camera, there is now more room for the image on the display.iPhone 13 has up to 2.5 hours longer battery life.The A15 Bionic processor and TrueDepth camera also power Face ID, an incredibly secure authentication technology.The ultra - fast A15 Bionic chip powers Cinema Effect, Photo Styles and more.Secure Enclave protects personal data, including Face ID and contacts.And the new chip increases the battery life.",
                DescTm = "OLED ekrany 28 % ýagtylandy - 800 cd / m² çenli.Everythinghli zat iň güneşli günlerde - de aýdyň görünýär.HDR - de mazmuny göreniňde ýagtylygy 1200 cd / m² ýetýär.Gara we ak reňkleriň iň kiçi kölegelerini - beýleki reňkler ýaly tapawutlandyryp bilersiňiz.Şol bir wagtyň özünde, displeý batareýanyň güýjüni öňküsinden has tygşytly sarp edýär. “Super Retina XDR” displeýinde ajaýyp ýokary piksel dykyzlygy bar, suratlar, wideolar we tekst ajaýyp görünýär. “TrueDepth” kamerasynyň kiçi meýdany sebäpli indi ekranda şekil üçin has köp ýer bar. “iPhone 13” -iň batareýasynyň ömri 2, 5 sagada çenli.A15 Bionic prosessor we TrueDepth kamerasy, şeýle hem ygtybarly ygtybarly tanamak tehnologiýasy Face ID - i güýçlendirýär.Ultra çalt A15 Bionic çipi Kino effekti, Surat stilleri we ş.m.Howpsuz Enklaw, Face ID we aragatnaşyklary goşmak bilen şahsy maglumatlary goraýar.Täze çip bolsa batareýanyň ömrüni artdyrýar."
            },
            new Model
            {
                Id = 5,
                Name = "B2152B",
                BrandId = 6,
                LineId = null,
                CategoryId = 178,
                TypeId = 5,
                WarrantyId = 1,
                DescRu = "Запатентованная система SmartFit позволяет настраивать высоту и угол экрана вашего ноутбука с помощью прилагаемой ручной диаграммы, чтобы найти свой личный цвет комфорта, уменьшая напряжение шеи и напряжение глаз. Easy Riser снимает ваш ноутбук со своего рабочего стола, способствуя воздушному потоку, чтобы улучшить работу аккумулятора и облегчить нагрузку на внутренние компоненты.",
                DescEn = "The patented SmartFit system allows you to adjust the height and angle of your laptop screen using the included hand chart to find your personal comfort color while reducing neck strain and eye strain.The Easy Riser lifts your laptop off its desktop, promoting airflow to improve battery performance and ease stress on internal components.",
                DescTm = "Patentlenen SmartFit ulgamy, boýnuň süzülmesini we gözüň dartylmagyny azaltmak bilen şahsy rahatlyk reňkini tapmak üçin goşulan el diagrammasyny ulanyp, noutbuk ekranyňyzyň beýikligini we burçuny sazlamaga mümkinçilik berýär. “Easy Riser”, batareýanyň işleýşini gowulandyrmak we içerki böleklere edilýän stresleri ýeňilleşdirmek üçin noutbukyňyzy iş stolundan çykarýar."
            },
            new Model
            {
                Id = 6,
                Name = "Z Fold 4",
                BrandId = 2,
                LineId = 4,
                CategoryId = 11,
                TypeId = 1,
                WarrantyId = 1,
                DescRu = "Samsung Galaxy Fold — смартфон, меняющий наше представление о смартфонах. Девайс сочетает огромный складной экран с исключительно мощным «железом». Многооконный режим отображает сразу 3 приложения. 7 - нанометровый процессор Snapdragon 855 усилен 12 ГБ оперативной памяти.Флэш - накопитель 512 ГБ построен на молниеносных чипах UFS 3.0.Популярные 3D - игры — PUBG, WoT: Blitz, Asphalt 9 и другие — здесь просто «летают». Удовольствие максимальное.Уникальный механизм позволят экрану складываться.Надежность шарнира проверена 200 000 раз.Девайс открывается естественно, плавно — как книга.Раскрытое положение четко фиксируется.Соединительный модуль превращает этот смартфон в компактный гаджет, который удобно носить с собой.Samsung Galaxy Fold — завтрашний день технологий, доступный уже сегодня.",
                DescEn = "The Samsung Galaxy Fold is a smartphone that is changing the way we think about smartphones. The device combines a huge foldable screen with exceptionally powerful hardware.Multi - window mode displays 3 applications at once.The 7nm Snapdragon 855 processor is boosted by 12GB of RAM.The 512 GB flash drive is built with lightning - fast UFS 3.0 chips.Popular 3D games - PUBG, WoT: Blitz, Asphalt 9 and others - just “fly” here.Maximum pleasure.The unique mechanism will allow the screen to be folded.The reliability of the hinge has been tested 200,000 times.The device opens naturally,smoothly - like a book.The open position is clearly fixed. The connector module turns this smartphone into a compact gadget that is easy to carry around. Samsung Galaxy Fold is tomorrow's technology, available today.	Samsung Galaxy Fold, smartfonlar baradaky pikirimizi üýtgedýän smartfondyr.",
                DescTm = "Enjam ullakan bukulýan ekrany gaty güýçli enjam bilen birleşdirýär. Köp penjire re modeimi birbada 3 programmany görkezýär. 7nm Snapdragon 855 prosessor 12 Gb RAM bilen güýçlendirilýär. 512 Gb fleş disk ýyldyrym çalt UFS 3.0 çipleri bilen gurlupdyr. Meşhur 3D oýunlary - PUBG, WoT: Blits, Asfalt 9 we başgalar - diňe şu ýerde “uçuň”. Iň ýokary lezzet. Üýtgeşik mehanizm ekrany bukmaga mümkinçilik berer. Çeňňegiň ygtybarlylygy 200,000 gezek synag edildi. Enjam kitap ýaly tebigy, rahat açylýar. Açyk pozisiýa anyk kesgitlenendir. Birikdiriji modul, bu smartfony daşamak aňsat bolan ykjam gadgeta öwürýär. Samsung Galaxy Fold, şu gün elýeterli ertirki tehnologiýa."
            });
            builder.Entity<ModelSpec>().HasData(
                new ModelSpec
                {
                    ModelId = 1,
                    SpecId = 1,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 1,
                    SpecId = 2,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 1,
                    SpecId = 3,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 1,
                    SpecId = 4,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 1,
                    SpecId = 5,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 2,
                    SpecId = 1,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 2,
                    SpecId = 2,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 2,
                    SpecId = 3,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 2,
                    SpecId = 4,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 2,
                    SpecId = 5,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 3,
                    SpecId = 1,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 3,
                    SpecId = 2,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 3,
                    SpecId = 3,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 3,
                    SpecId = 4,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 3,
                    SpecId = 5,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 4,
                    SpecId = 1,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 4,
                    SpecId = 2,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 4,
                    SpecId = 3,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 4,
                    SpecId = 4,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 4,
                    SpecId = 5,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 5,
                    SpecId = 1,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 5,
                    SpecId = 4,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 5,
                    SpecId = 5,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 6,
                    SpecId = 1,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 6,
                    SpecId = 2,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 6,
                    SpecId = 3,
                    IsNameUse = true
                },
                new ModelSpec
                {
                    ModelId = 6,
                    SpecId = 4,
                    IsNameUse = false
                },
                new ModelSpec
                {
                    ModelId = 6,
                    SpecId = 5,
                    IsNameUse = false
                });
            builder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Order = 0,
                ParentId = 0,
                NameRu = "Оргтехника",
                NameEn = "Office equipment",
                NameTm = "Ofis tehnikasy",
                IsForHome = true,
                NotInUse = false
            },
                        new Category
                        {
                            Id = 2,
                            Order = 1,
                            ParentId = 0,
                            NameRu = "Мобильные устройства",
                            NameEn = "Mobile devices",
                            NameTm = "Mobil gurluşlar",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 3,
                            Order = 2,
                            ParentId = 0,
                            NameRu = "Бытовая техника",
                            NameEn = "Home equipment",
                            NameTm = "Öy tehnikasy",
                            IsForHome = true,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 4,
                            Order = 3,
                            ParentId = 0,
                            NameRu = "Одежда и текстиль",
                            NameEn = "Clothing and textiles",
                            NameTm = "Eşik we tekstil",
                            IsForHome = true,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 5,
                            Order = 4,
                            ParentId = 0,
                            NameRu = "Еда",
                            NameEn = "Food",
                            NameTm = "Iýmit",
                            IsForHome = true,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 6,
                            Order = 0,
                            ParentId = 1,
                            NameRu = "Компьютеры",
                            NameEn = "Computers",
                            NameTm = "Kompýuterler",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 7,
                            Order = 1,
                            ParentId = 1,
                            NameRu = "Компьютерные комплектующие",
                            NameEn = "Computer parts",
                            NameTm = "Kompýuter enjamlary",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 8,
                            Order = 0,
                            ParentId = 6,
                            NameRu = "Моноблоки",
                            NameEn = "Monobloks",
                            NameTm = "Monobloklar",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 9,
                            Order = 1,
                            ParentId = 6,
                            NameRu = "Ноутбуки",
                            NameEn = "Notebooks",
                            NameTm = "Noutbuklar",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 10,
                            Order = 0,
                            ParentId = 2,
                            NameRu = "Телефоны, планшеты и аксессуары",
                            NameEn = "Phones, tabs & accessories",
                            NameTm = "Telefonlar, planşetler we aksesuarlar",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 11,
                            Order = 0,
                            ParentId = 10,
                            NameRu = "Мобильные телефоны",
                            NameEn = "Mobile phones",
                            NameTm = "Öyjükli telefonlar",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 12,
                            Order = 1,
                            ParentId = 10,
                            NameRu = "Планшеты",
                            NameEn = "Tablets",
                            NameTm = "Planşetler",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 13,
                            Order = 2,
                            ParentId = 6,
                            NameRu = "Брендовые компьютеры",
                            NameEn = "Brand PCs",
                            NameTm = "Brend kompýuterler",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 17,
                            Order = 2,
                            ParentId = 1,
                            NameRu = "Аксессуары для компьютеров",
                            NameEn = "PC accessories",
                            NameTm = "Kompýuter esbaplary",
                            IsForHome = false,
                            NotInUse = false
                        },
                        new Category
                        {
                            Id = 15,
                            Order = 0,
                            ParentId = 17,
                            NameRu = "Подставки для ноутбуков",
                            NameEn = "Notebook stands",
                            NameTm = "Noutbuk goltuklary",
                            IsForHome = false,
                            NotInUse = false
                        });
            builder.Entity<Spec>().HasData(
            new Spec
            {
                Id = 1,
                NameRu = "Цвет",
                NameEn = "Color",
                NameTm = "Reňk",
                IsFilter = true,
                IsImaged = true,
                Order = 1,
                NamingOrder = 2
            },
            new Spec
            {
                Id = 2,
                NameRu = "Память",
                NameEn = "Memory",
                NameTm = "Ýat",
                IsFilter = true,
                Order = 2
            },
            new Spec
            {
                Id = 3,
                NameRu = "Накопитель",
                NameEn = "Storage",
                NameTm = "Ýatda saklaýjy",
                IsFilter = true,
                Order = 3,
                NamingOrder = 1
            },
            new Spec
            {
                Id = 4,
                NameRu = "Вес",
                NameEn = "Weight",
                NameTm = "Agram",
                Order = 4
            },
            new Spec
            {
                Id = 5,
                NameRu = "Материал",
                NameEn = "Material",
                NameTm = "Material",
                Order = 0
            });
            builder.Entity<SpecsValue>().HasData(
            new SpecsValue
            {
                Id = 1,
                NameRu = "Черный",
                NameEn = "Black",
                NameTm = "Gara",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 2,
                NameRu = "Белый",
                NameEn = "White",
                NameTm = "Ak",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 3,
                NameRu = "Синий",
                NameEn = "Blue",
                NameTm = "Gök",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 4,
                NameRu = "Красный",
                NameEn = "Red",
                NameTm = "Gyzyl",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 5,
                NameRu = "Зеленый",
                NameEn = "Green",
                NameTm = "Ýaşyl",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 6,
                NameRu = "Серый",
                NameEn = "Gray",
                NameTm = "Çal",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 7,
                NameRu = "Желтый",
                NameEn = "Yellow",
                NameTm = "Sary",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 8,
                NameRu = "Золотой",
                NameEn = "Gold",
                NameTm = "Altynsow",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 9,
                NameRu = "Серебристый",
                NameEn = "Silver",
                NameTm = "Kümüşsow",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 10,
                NameRu = "Коричневый",
                NameEn = "Brown",
                NameTm = "Goňur",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 11,
                NameRu = "2ГБ",
                NameEn = "2GB",
                NameTm = "2GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 12,
                NameRu = "4ГБ",
                NameEn = "4GB",
                NameTm = "4GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 13,
                NameRu = "6ГБ",
                NameEn = "6GB",
                NameTm = "6GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 14,
                NameRu = "8ГБ",
                NameEn = "8GB",
                NameTm = "8GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 15,
                NameRu = "12ГБ",
                NameEn = "12GB",
                NameTm = "12GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 16,
                NameRu = "16ГБ",
                NameEn = "16GB",
                NameTm = "16GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 17,
                NameRu = "32ГБ",
                NameEn = "32GB",
                NameTm = "32GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 18,
                NameRu = "64ГБ",
                NameEn = "64GB",
                NameTm = "64GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 19,
                NameRu = "128ГБ",
                NameEn = "128GB",
                NameTm = "128GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 20,
                NameRu = "256ГБ",
                NameEn = "256GB",
                NameTm = "256GB",
                SpecId = 2
            },
            new SpecsValue
            {
                Id = 21,
                NameRu = "8ГБ",
                NameEn = "8GB",
                NameTm = "8GB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 22,
                NameRu = "16ГБ",
                NameEn = "16GB",
                NameTm = "16GB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 23,
                NameRu = "32ГБ",
                NameEn = "32GB",
                NameTm = "32GB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 24,
                NameRu = "64ГБ",
                NameEn = "64GB",
                NameTm = "64GB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 25,
                NameRu = "128ГБ",
                NameEn = "128GB",
                NameTm = "128GB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 26,
                NameRu = "256ГБ",
                NameEn = "256GB",
                NameTm = "256GB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 27,
                NameRu = "512ГБ",
                NameEn = "512GB",
                NameTm = "512GB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 28,
                NameRu = "1ТБ",
                NameEn = "1TB",
                NameTm = "1TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 29,
                NameRu = "2ТБ",
                NameEn = "2TB",
                NameTm = "2TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 30,
                NameRu = "3ТБ",
                NameEn = "3TB",
                NameTm = "3TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 31,
                NameRu = "4ТБ",
                NameEn = "4TB",
                NameTm = "4TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 32,
                NameRu = "5ТБ",
                NameEn = "5TB",
                NameTm = "5TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 33,
                NameRu = "6ТБ",
                NameEn = "6TB",
                NameTm = "6TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 34,
                NameRu = "8ТБ",
                NameEn = "8TB",
                NameTm = "8TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 35,
                NameRu = "10ТБ",
                NameEn = "10TB",
                NameTm = "10TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 36,
                NameRu = "12ТБ",
                NameEn = "12TB",
                NameTm = "12TB",
                SpecId = 3
            },
            new SpecsValue
            {
                Id = 37,
                NameRu = "144гр",
                NameEn = "144g",
                NameTm = "144g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 38,
                NameRu = "212гр",
                NameEn = "212g",
                NameTm = "212g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 39,
                NameRu = "570гр",
                NameEn = "570g",
                NameTm = "570g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 40,
                NameRu = "1.1кг",
                NameEn = "1.1kg",
                NameTm = "1.1kg",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 41,
                NameRu = "174гр",
                NameEn = "174g",
                NameTm = "174g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 42,
                NameRu = "Розовый",
                NameEn = "Pink",
                NameTm = "Gülgüne",
                SpecId = 1
            },
            new SpecsValue
            {
                Id = 43,
                NameRu = "203гр",
                NameEn = "203g",
                NameTm = "203g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 44,
                NameRu = "238гр",
                NameEn = "238g",
                NameTm = "238g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 45,
                NameRu = "Пластик",
                NameEn = "Plastic",
                NameTm = "Plastik",
                SpecId = 5
            },
            new SpecsValue
            {
                Id = 46,
                NameRu = "Металл",
                NameEn = "Metal",
                NameTm = "Metal",
                SpecId = 5
            },
            new SpecsValue
            {
                Id = 47,
                NameRu = "Стекло",
                NameEn = "Glass",
                NameTm = "Aýna",
                SpecId = 5
            },
            new SpecsValue
            {
                Id = 48,
                NameRu = "Металл и стекло",
                NameEn = "Metal and glass",
                NameTm = "Metal we aýna",
                SpecId = 5
            },
            new SpecsValue
            {
                Id = 49,
                NameRu = "Металл и пластик",
                NameEn = "Metal and plastic",
                NameTm = "Metal we plastik",
                SpecId = 5
            },
            new SpecsValue
            {
                Id = 50,
                NameRu = "850гр",
                NameEn = "850g",
                NameTm = "850g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 51,
                NameRu = "950гр",
                NameEn = "950g",
                NameTm = "950g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 52,
                NameRu = "263гр",
                NameEn = "263g",
                NameTm = "263g",
                SpecId = 4
            },
            new SpecsValue
            {
                Id = 53,
                NameRu = "Бежевый",
                NameEn = "Beige",
                NameTm = "Bej",
                SpecId = 1
            });
            builder.Entity<SpecsValueMod>().HasData(
            new SpecsValueMod
            {
                Id = 1,
                SpecsValueId = 1,
                NameRu = "Полночь",
                NameEn = "Midnight",
                NameTm = "Ýary gije",
            },
            new SpecsValueMod
            {
                Id = 2,
                SpecsValueId = 2,
                NameRu = "Звездный свет",
                NameEn = "Starlight",
                NameTm = "Ýyldyz yşky",
            },
            new SpecsValueMod
            {
                Id = 3,
                SpecsValueId = 6,
                NameRu = "Графитовый",
                NameEn = "Graphite",
                NameTm = "Grafit"
            },
            new SpecsValueMod
            {
                Id = 4,
                SpecsValueId = 6,
                NameRu = "Серо-зеленый",
                NameEn = "Graygreen",
                NameTm = "Çal-ýaşyl"
            },
            new SpecsValueMod
            {
                Id = 5,
                SpecsValueId = 1,
                NameRu = "Фантомный черный",
                NameEn = "Phantom Black",
                NameTm = "Fantom gara"
            });
            builder.Entity<Warranty>().HasData(
            new Warranty
            {
                Id = 1,
                NameRu = "1 неделя",
                NameEn = "1 week",
                NameTm = "1 hepde"
            },
            new Warranty
            {
                Id = 2,
                NameRu = "1 месяц",
                NameEn = "1 month",
                NameTm = "1 aý"
            },
            new Warranty
            {
                Id = 3,
                NameRu = "3 месяца",
                NameEn = "3 months",
                NameTm = "3 aý"
            },
            new Warranty
            {
                Id = 4,
                NameRu = "6 месяцев",
                NameEn = "6 months",
                NameTm = "6 aý"
            },
            new Warranty
            {
                Id = 5,
                NameRu = "1 год",
                NameEn = "1 year",
                NameTm = "1 ýyl"
            });
            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    PartNo = null,
                    Price = 440.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 1
                },
                new Product
                {
                    Id = 2,
                    PartNo = null,
                    Price = 440.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 1
                },
                new Product
                {
                    Id = 3,
                    PartNo = null,
                    Price = 840.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 4
                },
                new Product
                {
                    Id = 4,
                    PartNo = null,
                    Price = 840.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 4
                },
                new Product
                {
                    Id = 5,
                    PartNo = null,
                    Price = 840.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 4
                },
                new Product
                {
                    Id = 6,
                    PartNo = null,
                    Price = 1050.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 2
                },
                new Product
                {
                    Id = 7,
                    PartNo = null,
                    Price = 1050.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 2
                },
                new Product
                {
                    Id = 8,
                    PartNo = null,
                    Price = 1050.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 2
                },
                new Product
                {
                    Id = 9,
                    PartNo = null,
                    Price = 1050.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 2
                },
                new Product
                {
                    Id = 10,
                    PartNo = null,
                    Price = 1155.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 2
                },
                new Product
                {
                    Id = 11,
                    PartNo = null,
                    Price = 1155.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 2
                },
                new Product
                {
                    Id = 12,
                    PartNo = null,
                    Price = 1155.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 2
                },
                new Product
                {
                    Id = 13,
                    PartNo = null,
                    Price = 1155.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 2
                },
                new Product
                {
                    Id = 14,
                    PartNo = null,
                    Price = 1150.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 3
                },
                new Product
                {
                    Id = 15,
                    PartNo = null,
                    Price = 1150.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 3
                },
                new Product
                {
                    Id = 16,
                    PartNo = null,
                    Price = 1150.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 3
                },
                new Product
                {
                    Id = 17,
                    PartNo = null,
                    Price = 1150.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 3
                },
                new Product
                {
                    Id = 18,
                    PartNo = null,
                    Price = 1255.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 3
                },
                new Product
                {
                    Id = 19,
                    PartNo = null,
                    Price = 1255.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 3
                },
                new Product
                {
                    Id = 20,
                    PartNo = null,
                    Price = 1255.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 3
                },
                new Product
                {
                    Id = 21,
                    PartNo = null,
                    Price = 1255.00M,
                    NewPrice = null,
                    NotInUse = false,
                    IsRecommended = false,
                    IsNew = false,
                    OnOrder = false,
                    ModelId = 3
                },
                new Product
                {
                    Id = 22,
                    PartNo = null,
                    Price = 85.00M,
                    NewPrice = 80.00M,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 5
                },
                new Product
                {
                    Id = 23,
                    PartNo = null,
                    Price = 1680.00M,
                    NewPrice = 1600.00M,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 6
                },
                new Product
                {
                    Id = 24,
                    PartNo = null,
                    Price = 1680.00M,
                    NewPrice = 1600.00M,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 6
                },
                new Product
                {
                    Id = 25,
                    PartNo = null,
                    Price = 1680.00M,
                    NewPrice = 1600.00M,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 6
                },
                new Product
                {
                    Id = 26,
                    PartNo = null,
                    Price = 1580.00M,
                    NewPrice = 1500.00M,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 6
                },
                new Product
                {
                    Id = 27,
                    PartNo = null,
                    Price = 1580.00M,
                    NewPrice = 1500.00M,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 6
                },
                new Product
                {
                    Id = 28,
                    PartNo = null,
                    Price = 1580.00M,
                    NewPrice = 1500.00M,
                    NotInUse = false,
                    IsRecommended = true,
                    IsNew = true,
                    OnOrder = false,
                    ModelId = 6
                });
            builder.Entity<ProductSpecsValue>().HasData(
                new ProductSpecsValue
                {
                    ProductId = 1,
                    SpecsValueId = 2
                },
                new ProductSpecsValue
                {
                    ProductId = 1,
                    SpecsValueId = 12
                },
                new ProductSpecsValue
                {
                    ProductId = 1,
                    SpecsValueId = 24
                },
                new ProductSpecsValue
                {
                    ProductId = 1,
                    SpecsValueId = 37
                },
                new ProductSpecsValue
                {
                    ProductId = 1,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 2,
                    SpecsValueId = 1
                },
                new ProductSpecsValue
                {
                    ProductId = 2,
                    SpecsValueId = 12
                },
                new ProductSpecsValue
                {
                    ProductId = 2,
                    SpecsValueId = 24
                },
                new ProductSpecsValue
                {
                    ProductId = 2,
                    SpecsValueId = 37
                },
                new ProductSpecsValue
                {
                    ProductId = 2,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 3,
                    SpecsValueId = 1
                },
                new ProductSpecsValue
                {
                    ProductId = 3,
                    SpecsValueId = 12
                },
                new ProductSpecsValue
                {
                    ProductId = 3,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 3,
                    SpecsValueId = 41
                },
                new ProductSpecsValue
                {
                    ProductId = 3,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 4,
                    SpecsValueId = 2
                },
                new ProductSpecsValue
                {
                    ProductId = 4,
                    SpecsValueId = 12
                },
                new ProductSpecsValue
                {
                    ProductId = 4,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 4,
                    SpecsValueId = 41
                },
                new ProductSpecsValue
                {
                    ProductId = 4,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 5,
                    SpecsValueId = 12
                },
                new ProductSpecsValue
                {
                    ProductId = 5,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 5,
                    SpecsValueId = 41
                },
                new ProductSpecsValue
                {
                    ProductId = 5,
                    SpecsValueId = 42
                },
                new ProductSpecsValue
                {
                    ProductId = 5,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 6,
                    SpecsValueId = 6
                },
                new ProductSpecsValue
                {
                    ProductId = 6,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 6,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 6,
                    SpecsValueId = 43
                },
                new ProductSpecsValue
                {
                    ProductId = 6,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 7,
                    SpecsValueId = 5
                },
                new ProductSpecsValue
                {
                    ProductId = 7,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 7,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 7,
                    SpecsValueId = 43
                },
                new ProductSpecsValue
                {
                    ProductId = 7,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 8,
                    SpecsValueId = 9
                },
                new ProductSpecsValue
                {
                    ProductId = 8,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 8,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 8,
                    SpecsValueId = 43
                },
                new ProductSpecsValue
                {
                    ProductId = 8,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 9,
                    SpecsValueId = 8
                },
                new ProductSpecsValue
                {
                    ProductId = 9,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 9,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 9,
                    SpecsValueId = 43
                },
                new ProductSpecsValue
                {
                    ProductId = 9,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 10,
                    SpecsValueId = 6
                },
                new ProductSpecsValue
                {
                    ProductId = 10,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 10,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 10,
                    SpecsValueId = 43
                },
                new ProductSpecsValue
                {
                    ProductId = 10,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 11,
                    SpecsValueId = 5
                },
                new ProductSpecsValue
                {
                    ProductId = 11,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 11,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 11,
                    SpecsValueId = 43
                },
                new ProductSpecsValue
                {
                    ProductId = 11,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 12,
                    SpecsValueId = 9
                },
                new ProductSpecsValue
                {
                    ProductId = 12,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 12,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 12,
                    SpecsValueId = 43
                },
                new ProductSpecsValue
                {
                    ProductId = 12,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 13,
                    SpecsValueId = 8
                },
                new ProductSpecsValue
                {
                    ProductId = 13,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 13,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 13,
                    SpecsValueId = 43
                },
                new ProductSpecsValue
                {
                    ProductId = 13,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 14,
                    SpecsValueId = 6
                },
                new ProductSpecsValue
                {
                    ProductId = 14,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 14,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 14,
                    SpecsValueId = 44
                },
                new ProductSpecsValue
                {
                    ProductId = 14,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 15,
                    SpecsValueId = 5
                },
                new ProductSpecsValue
                {
                    ProductId = 15,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 15,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 15,
                    SpecsValueId = 44
                },
                new ProductSpecsValue
                {
                    ProductId = 15,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 16,
                    SpecsValueId = 9
                },
                new ProductSpecsValue
                {
                    ProductId = 16,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 16,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 16,
                    SpecsValueId = 44
                },
                new ProductSpecsValue
                {
                    ProductId = 16,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 17,
                    SpecsValueId = 8
                },
                new ProductSpecsValue
                {
                    ProductId = 17,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 17,
                    SpecsValueId = 25
                },
                new ProductSpecsValue
                {
                    ProductId = 17,
                    SpecsValueId = 44
                },
                new ProductSpecsValue
                {
                    ProductId = 17,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 18,
                    SpecsValueId = 6
                },
                new ProductSpecsValue
                {
                    ProductId = 18,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 18,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 18,
                    SpecsValueId = 44
                },
                new ProductSpecsValue
                {
                    ProductId = 18,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 19,
                    SpecsValueId = 5
                },
                new ProductSpecsValue
                {
                    ProductId = 19,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 19,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 19,
                    SpecsValueId = 44
                },
                new ProductSpecsValue
                {
                    ProductId = 19,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 20,
                    SpecsValueId = 9
                },
                new ProductSpecsValue
                {
                    ProductId = 20,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 20,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 20,
                    SpecsValueId = 44
                },
                new ProductSpecsValue
                {
                    ProductId = 20,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 21,
                    SpecsValueId = 8
                },
                new ProductSpecsValue
                {
                    ProductId = 21,
                    SpecsValueId = 13
                },
                new ProductSpecsValue
                {
                    ProductId = 21,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 21,
                    SpecsValueId = 44
                },
                new ProductSpecsValue
                {
                    ProductId = 21,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 22,
                    SpecsValueId = 1
                },
                new ProductSpecsValue
                {
                    ProductId = 22,
                    SpecsValueId = 45
                },
                new ProductSpecsValue
                {
                    ProductId = 22,
                    SpecsValueId = 50
                },
                new ProductSpecsValue
                {
                    ProductId = 23,
                    SpecsValueId = 6
                },
                new ProductSpecsValue
                {
                    ProductId = 23,
                    SpecsValueId = 15
                },
                new ProductSpecsValue
                {
                    ProductId = 23,
                    SpecsValueId = 27
                },
                new ProductSpecsValue
                {
                    ProductId = 23,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 23,
                    SpecsValueId = 52
                },
                new ProductSpecsValue
                {
                    ProductId = 24,
                    SpecsValueId = 1
                },
                new ProductSpecsValue
                {
                    ProductId = 24,
                    SpecsValueId = 15
                },
                new ProductSpecsValue
                {
                    ProductId = 24,
                    SpecsValueId = 27
                },
                new ProductSpecsValue
                {
                    ProductId = 24,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 24,
                    SpecsValueId = 52
                },
                new ProductSpecsValue
                {
                    ProductId = 25,
                    SpecsValueId = 53
                },
                new ProductSpecsValue
                {
                    ProductId = 25,
                    SpecsValueId = 15
                },
                new ProductSpecsValue
                {
                    ProductId = 25,
                    SpecsValueId = 27
                },
                new ProductSpecsValue
                {
                    ProductId = 25,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 25,
                    SpecsValueId = 52
                },
                new ProductSpecsValue
                {
                    ProductId = 26,
                    SpecsValueId = 6
                },
                new ProductSpecsValue
                {
                    ProductId = 26,
                    SpecsValueId = 15
                },
                new ProductSpecsValue
                {
                    ProductId = 26,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 26,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 26,
                    SpecsValueId = 52
                },
                new ProductSpecsValue
                {
                    ProductId = 27,
                    SpecsValueId = 1
                },
                new ProductSpecsValue
                {
                    ProductId = 27,
                    SpecsValueId = 15
                },
                new ProductSpecsValue
                {
                    ProductId = 27,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 27,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 27,
                    SpecsValueId = 52
                },
                new ProductSpecsValue
                {
                    ProductId = 28,
                    SpecsValueId = 53
                },
                new ProductSpecsValue
                {
                    ProductId = 28,
                    SpecsValueId = 15
                },
                new ProductSpecsValue
                {
                    ProductId = 28,
                    SpecsValueId = 26
                },
                new ProductSpecsValue
                {
                    ProductId = 28,
                    SpecsValueId = 48
                },
                new ProductSpecsValue
                {
                    ProductId = 28,
                    SpecsValueId = 52
                });
            builder.Entity<ProductSpecsValueMod>().HasData(
                new ProductSpecsValueMod
                {
                    ProductId = 2,
                    SpecsValueModId = 1
                },
                new ProductSpecsValueMod
                {
                    ProductId = 3,
                    SpecsValueModId = 1
                },
                new ProductSpecsValueMod
                {
                    ProductId = 4,
                    SpecsValueModId = 2
                },
                new ProductSpecsValueMod
                {
                    ProductId = 6,
                    SpecsValueModId = 3
                },
                new ProductSpecsValueMod
                {
                    ProductId = 23,
                    SpecsValueModId = 4
                },
                new ProductSpecsValueMod
                {
                    ProductId = 24,
                    SpecsValueModId = 5
                },
                new ProductSpecsValueMod
                {
                    ProductId = 26,
                    SpecsValueModId = 4
                },
                new ProductSpecsValueMod
                {
                    ProductId = 27,
                    SpecsValueModId = 5
                });
            builder.Entity<Slide>().HasData(
            new Slide
            {
                Id = 1,
                Name = "Скидки на сайте tolkuchka.bar",
                Layout = 0,
                Link = "#"
            },
            new Slide
            {
                Id = 2,
                Name = "Акции на сайте tolkuchka.bar",
                Layout = 0,
                Link = "#"
            });
        }
    }
}
