using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Random random = new Random();
        Supermarket supermarket = new Supermarket(random);
        supermarket.Work();
    }
}

class Supermarket
{
    private int _money = 0;
    private Queue<Client> _clients = new Queue<Client>();

    public Supermarket(Random random, int minClientMoneys = 100, int maxClientMoneys = 150, int clients = 13)
    {
        ProductsGenerator productGenerator = new ProductsGenerator();

        for (int i = 0; i < clients; i++)
        {
            int money = random.Next(minClientMoneys, maxClientMoneys);
            List<Product> products = productGenerator.Generete(random);
            _clients.Enqueue(new Client(products, money));
        }
    }

    public void Work()
    {
        while (_clients.Count > 0)
        {
            Client client = _clients.Peek();
            client.ShowInfo();

            if (client.TryBuyProductInBasket(out int money))
                _money += money;

            client.ShowInfo();
            Console.WriteLine(("  money supermarket: " + _money + "$"));
            Console.WriteLine();
            Console.ReadKey();
            _clients.Dequeue();
        }
    }
}

class Client
{
    private List<Product> _productsInBasket;
    private List<Product> _productsInBag = new List<Product>();

    public Client(List<Product> products, int money)
    {
        _productsInBasket = new List<Product>(products);
        Money = money;
    }

    public int Money { get; private set; }

    public bool TryBuyProductInBasket(out int money)
    {
        money = 0;

        while (CalculateSumm(_productsInBasket) > Money && _productsInBasket.Count > 0)
            _productsInBasket.Remove(_productsInBasket[0]);

        if (_productsInBasket.Count == 0)
            return false;

        Money -= CalculateSumm(_productsInBasket);
        money = CalculateSumm(_productsInBasket);
        _productsInBag.AddRange(_productsInBasket);
        _productsInBasket.Clear();
        return true;
    }

    public void ShowInfo()
    {
        ShowCollection(_productsInBasket);
        Console.WriteLine();
        ShowCollection(_productsInBag);
        Console.WriteLine("  money client: " + Money + "$");
    }

    private void ShowCollection(List<Product> products)
    {
        foreach (var product in products)
            product.ShowInfo();
    }

    private int CalculateSumm(List<Product> products)
    {
        int summ = 0;

        foreach (var product in products)
            summ += product.Price;

        return summ;
    }
}

class ProductsGenerator
{
    private List<Product> _products = new List<Product>();

    public ProductsGenerator()
    {
        _products = new List<Product>() { new Product("pr_0", 26),
                                          new Product("pr_1", 48),
                                          new Product("pr_2", 19),
                                          new Product("pr_3", 11),
                                          new Product("pr_4", 52),
                                          new Product("pr_5", 44),
                                          new Product("pr_6", 19) };
    }

    public List<Product> Generete(Random random, bool needCopyAllList = false)
    {
        List<Product> products = new List<Product>();
        int minQuantityProductsInList = 4;
        int maxQuantityProductsInList = _products.Count + 1;
        int quantityOfProducts;

        if (needCopyAllList)
            quantityOfProducts = _products.Count;
        else
            quantityOfProducts = random.Next(minQuantityProductsInList, maxQuantityProductsInList);

        for (int i = 0; i < quantityOfProducts; i++)
        {
            int randomIndexOfProduct = (random.Next(0, _products.Count));
            var product = _products[randomIndexOfProduct];
            products.Add(product.Clone());
            _products.RemoveAt(randomIndexOfProduct);
        }

        return products;
    }
}

class Product
{
    public Product(string title = "", int price = 0)
    {
        Title = title;
        Price = price;
    }

    public string Title { get; private set; }
    public int Price { get; private set; }

    public Product Clone()
    {
        return new Product(Title, Price);
    }

    public void ShowInfo()
    {
        Console.WriteLine(Title + " " + Price + "$" + " ");
    }
}