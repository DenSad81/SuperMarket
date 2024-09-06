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
    private List<Product> _products;

    public Supermarket(Random random)
    {
        int minQuantityClientMoneys = 100;
        int maxQuantityClientMoneys = 150;
        int quantityOfClients = 13;
        GeneratorOfProducts generatorOfProducts;

        for (int i = 0; i < quantityOfClients; i++)
        {
            int money = random.Next(minQuantityClientMoneys, maxQuantityClientMoneys);
            generatorOfProducts = new GeneratorOfProducts();
            List<Product> products = generatorOfProducts.GenereteNewRandomListOfProductsWithoutRepiting(random);
            _clients.Enqueue(new Client(products, money, random));
        }

        generatorOfProducts = new GeneratorOfProducts();
        _products = generatorOfProducts.GenereteNewRandomListOfProductsWithoutRepiting(random, true);
    }

    public void Work()
    {
        while (_clients.Count > 0)
        {
            Client client = _clients.Peek();
            client.ShowInfo();
            Product wontedProduct = client.GetProductWhichWontToBuy(_products);
            bool isMoneyEnauf;

            do
            {
                isMoneyEnauf = client.TryAddProductIntoBag(wontedProduct);

                if (isMoneyEnauf)
                {
                    _money += wontedProduct.Price;
                }
                else
                {
                    if (client.TryDeleteRandomProductInBasket() == false)
                        break;
                }
            }
            while (isMoneyEnauf == false);

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
    private Random _random;
    public int Money { get; private set; }

    public Client(List<Product> products, int money, Random random)
    {
        _productsInBasket = products;
        _random = random;
        Money = money;
    }

    public bool TryDeleteRandomProductInBasket()
    {
        if (_productsInBasket.Count > 0)
        {
            var product = _productsInBasket[_random.Next(0, _productsInBasket.Count)];
            Money += product.Price;
            _productsInBasket.Remove(product);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryAddProductIntoBag(Product product)
    {
        if (Money >= product.Price)
        {
            Money -= product.Price;
            _productsInBag.Add(product);
            return true;
        }
        else
        {
            return false;
        }
    }

    public Product GetProductWhichWontToBuy(List<Product> products)
    {
        return products[_random.Next(0, products.Count)].Clone();
    }

    public void ShowInfo()
    {
        foreach (var product in _productsInBasket)
            product.ShowInfo();

        Console.WriteLine();

        foreach (var product in _productsInBag)
            product.ShowInfo();

        Console.WriteLine("  money client: " + Money + "$");
    }
}

class GeneratorOfProducts
{
    private List<Product> _products = new List<Product>();

    public GeneratorOfProducts()
    {
        _products = new List<Product>() { new Product("pr_0", 26),
                                          new Product("pr_1", 48),
                                          new Product("pr_2", 19),
                                          new Product("pr_3", 11),
                                          new Product("pr_4", 52),
                                          new Product("pr_5", 44),
                                          new Product("pr_6", 19) };
    }

    public List<Product> GenereteNewRandomListOfProductsWithoutRepiting(Random random, bool allList = false)
    {
        List<Product> products = new List<Product>();
        int minQuantityProductsInList = 4;
        int maxQuantityProductsInList = _products.Count + 1;
        int quantityOfProducts;

        if (allList == true)
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
    public string Title { get; private set; }
    public int Price { get; private set; }

    public Product(string title = "", int price = 0)
    {
        Title = title;
        Price = price;
    }

    public Product Clone()
    {
        return new Product(Title, Price);
    }

    public void ShowInfo()
    {
        Console.WriteLine(Title + " " + Price + "$" + " ");
    }
}