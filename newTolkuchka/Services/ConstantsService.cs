using newTolkuchka.Models;

namespace newTolkuchka.Services
{
    public static class ConstantsService
    {
        public static string UserPinKey(int id) => $"{USER}{id}";
        public static string UserHashKey(int id) => $"{USER}{id}{HASH}";
        public static string EmpHashKey(int id) => $"{EMPLOYEE}{id}{HASH}";
        // lang standarts
        public const string CULTURE = "culture";
        public const string RU = "ru";
        public const string EN = "en";
        public const string TK = "tk";
        public const string TM = "tm";
        public const string RUST = "ru_RU";
        public const string ENST = "en_GB";
        public const string TMST = "tk_TM";
        public const string CURRENTPHONE = "+99365 561427";
        public const string INFOEMAIL = "info@";
        // viewdatas
        public const int MOBILEWIDTH = 992;
        public const int PHONEWIDTH = 768;
        public const string TITLE = "title";
        public const string DESCRIPTION = "description";
        public const string SPECIFICATIONS = "specifications";
        public const string URL = "url";
        public const string IMAGE = "image";
        public const string NAME = "name";
        public const string PHONE = "phone";
        public const string EMAIL = "email";
        public const string NAME2 = "name2";
        public const string ADDRESS = "address";
        public const string TEXT = "text";
        public const string NUMBER = "number";
        public const string PASSWORD = "password";
        public const string DATE = "date";
        public const string REQUIRED = "required";
        public const string ENTER = "enter";
        public const string NEXT = "next";
        public const string EMPTY = "empty";
        public const string ORDER = "order";
        public const string BIRTHDAY = "birthday";
        public const string PIN = "pin";
        // routes models
        public const string SLASH = "/";
        public const string HOME = "home";
        public const string INDEX = "index";
        public const string ARTICLES = "articles";
        public const string ARTICLE = "article";
        public const string BRANDS = "brands";
        public const string BRAND = "brand";
        public const string CATEGORIES = "categories";
        public const string CATEGORY = "category";
        public const string CURRENCIES = "currencies";
        public const string CURRENCY = "currency";
        public const string EMPLOYEES = "employees";
        public const string EMPLOYEE = "employee";
        public const string ENTRIES = "entries";
        public const string ENTRY = "entry";
        public const string HEADINGS = "headings";
        public const string HEADING = "heading";
        public const string INVOICES = "invoices";
        public const string INVOICE = "invoice";
        public const string LINES = "lines";
        public const string LINE = "line";
        public const string MODELS = "models";
        public const string MODEL = "model";
        public const string POSITIONS = "positions";
        public const string POSITION = "position";
        public const string PRODUCTS = "products";
        public const string PRODUCT = "product";
        public const string PROMOTIONS = "promotions";
        public const string PROMOTION = "promotion";
        public const string PURCHASEINVOICES = "purchaseinvoices";
        public const string PURCHASEINVOICE = "purchaseinvoice";
        public const string PURCHASES = "purchases";
        public const string PURCHASE = "purchase";
        public const string REPORT = "report";
        public const string SLIDES = "slides";
        public const string SLIDE = "slide";
        public const string SPECS = "specs";
        public const string SPEC = "spec";
        public const string SPECSVALUEMODS = "specsvaluemods";
        public const string SPECSVALUEMOD = "specsvaluemod";
        public const string SPECSVALUES = "specsvalues";
        public const string SPECSVALUE = "specsvalue";
        public const string SUPPLIERS = "suppliers";
        public const string SUPPLIER = "supplier";
        public const string TYPES = "types";
        public const string TYPE = "type";
        public const string WARRANTIES = "warranties";
        public const string WARRANTY = "warranty";
        public const string ACCOUNT = "account";
        public const string LOGIN = "login";
        public const string CART = "cart";
        public const string SEARCH = "search";
        public const string ABOUT = "about";
        public const string DELIVERY = "delivery";
        public const string MENU = "menu";
        public const string NOVELTIES = "novelties";
        public const string RECOMMENDED = "recommended";
        public const string LIKED = "liked";
        public const string NEW = "new";
        public const string START = "start";
        public const string END = "end";
        public const string NOTINUSE = "notinuse";
        public const string N404 = "404";
        public const string USER = "user";
        public const string HASH = "hash";
        // images
        public const int PRODUCTMAXIMAGE = 5;
        public const int UMAXIMAGE = 1;
        public const int LOCALMAXIMAGE = 3;
        //other stuff
        public const int ARTICLEMAXLENGTH = 4000;
        public const int DELIVERYFREE = 500;
        public const int DELIVERYPRICE = 20;
    }
}
