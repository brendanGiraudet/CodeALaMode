using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        // customers list
        var customerList = GetCustomers();
        customerList.ForEach(c =>
        {
            Console.Error.WriteLine(c);
        });

        //kitchen
        var kitchen = GetKitchen();
        Console.Error.WriteLine(kitchen);

        // game loop
        while (true)
        {
            int turnsRemaining = int.Parse(Console.ReadLine());
            // me
            var me = GetCooker();
            Console.Error.WriteLine(me);
            // partner
            var partner = GetCooker();
            // table list
            var tableList = GetTables();
            tableList.ForEach(t =>
            {
                Console.Error.WriteLine(t);
            });

            // Pas dans cette ligue
            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            string ovenContents = inputs[0]; // ignore until wood 1 league
            int ovenTimer = int.Parse(inputs[1]);
            int numCustomers = int.Parse(Console.ReadLine()); // the number of customers currently waiting for food
            for (int i = 0; i < numCustomers; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                string customerItem = inputs[0];
                int customerAward = int.Parse(inputs[1]);
            }

            // reponse
            string reponse = "";

            // récupération commande
            var order = customerList.First();

            
            if (me.HaveItem())
            {
                //locate window
                if(me.HaveDishBlueberryIcecream())
                {
                    var window = kitchen.GetWindowLocation();
                    Console.Error.WriteLine(window);
                    if (window != null)
                    {
                        reponse = "USE " + window.X + " " + window.Y;
                    }
                }
                else
                {
                    // locate icecream
                    if (me.HaveDishBlueberry())
                    {
                        var icecream = kitchen.GetIcecreamLocation();
                        Console.Error.WriteLine(icecream);
                        if (icecream != null)
                        {
                            reponse = "USE " + icecream.X + " " + icecream.Y;
                        }
                    }
                    else
                    {
                        // locate blueberries
                        var blueberry = kitchen.GetBlueberriesLocation();
                        Console.Error.WriteLine(blueberry);
                        if (blueberry != null)
                        {
                            reponse = "USE " + blueberry.X + " " + blueberry.Y;
                        }
                    }
                }
            }
            else
            {
                // locate dish
                var dish = kitchen.GetDishLocation();
                if (dish != null)
                {
                    reponse = "USE " + dish.X + " " + dish.Y;
                }
            }

            // si rien à faire alors attendre
            if (string.IsNullOrEmpty(reponse))
            {
                reponse = "WAIT";
            }

            // MOVE x y
            // USE x y
            // WAIT

            Console.WriteLine(reponse);
        }
    }
    private static Kitchen GetKitchen()
    {
        var kitchen = new Kitchen();
        for (int i = 0; i < 7; i++)
        {
            kitchen.Lines.Add(Console.ReadLine());
        }
        return kitchen;
    }
    private static List<Customer> GetCustomers()
    {
        string[] inputs;
        var list = new List<Customer>();
        int numAllCustomers = int.Parse(Console.ReadLine());
        for (int i = 0; i < numAllCustomers; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            list.Add(new Customer
            {
                Item = inputs[0],
                Award = int.Parse(inputs[1])
            });
        }
        return list;
    }
    private static Cooker GetCooker()
    {
        var inputs = Console.ReadLine().Split(' ');
        return new Cooker
        {
            Location = new Location
            {
                X = int.Parse(inputs[0]),
                Y = int.Parse(inputs[1])
            },
            Item = inputs[2]
        };
    }
    private static List<Table> GetTables()
    {
        var list = new List<Table>();
        string[] inputs;
        int numTablesWithItems = int.Parse(Console.ReadLine()); // the number of tables in the kitchen that currently hold an item
        for (int i = 0; i < numTablesWithItems; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            list.Add(new Table
            {
                Location = new Location
                {
                    X = int.Parse(inputs[0]),
                    Y = int.Parse(inputs[1])
                },
                Item = inputs[2]
            });
        }
        return list;
    }
}
public class Kitchen
{
    public List<string> Lines { get; set; } = new List<string>();

    public Location GetDishLocation()
    {
        for (int i = 0; i < Lines.Count(); i++)
        {
            for (int j = 0; j < Lines[i].Count(); j++)
            {
                if (Lines[i][j].Equals('D'))
                {
                    return new Location
                    {
                        X = j,
                        Y = i
                    };
                }
            }
        }
        return null;
    }

    public Location GetBlueberriesLocation()
    {
        for (int i = 0; i < Lines.Count(); i++)
        {
            for (int j = 0; j < Lines[i].Count(); j++)
            {
                if (Lines[i][j].Equals('B'))
                {
                    return new Location
                    {
                        X = j,
                        Y = i
                    };
                }
            }
        }
        return null;
    }

    public Location GetIcecreamLocation()
    {
        for (int i = 0; i < Lines.Count(); i++)
        {
            for (int j = 0; j < Lines[i].Count(); j++)
            {
                if (Lines[i][j].Equals('I'))
                {
                    return new Location
                    {
                        X = j,
                        Y = i
                    };
                }
            }
        }
        return null;
    }

    public Location GetWindowLocation()
    {
        for (int i = 0; i < Lines.Count(); i++)
        {
            for (int j = 0; j < Lines[i].Count(); j++)
            {
                if (Lines[i][j].Equals('W'))
                {
                    return new Location
                    {
                        X = j,
                        Y = i
                    };
                }
            }
        }
        return null;
    }

    public override string ToString()
    {
        return string.Join("  \n", Lines);
    }
}
public class Customer
{
    public int Award { get; set; }
    public string Item { get; set; }

    public override string ToString()
    {
        return "Customer award :" + this.Award + " item:" + this.Item;
    }
}
public class Cooker
{
    public Location Location { get; set; } = new Location();
    public string Item { get; set; }

    public bool HaveDish()
    {
        return Item.Equals("DISH");
    }

    public bool HaveItem()
    {
        return (HaveDish() || HaveDishBlueberry() || HaveDishBlueberryIcecream());
    }

    public bool HaveDishBlueberry()
    {
        return Item.Equals("DISH-BLUEBERRIES");
    }

    public bool HaveDishIcecream()
    {
        return Item.Equals("DISH-ICE_CREAM");
    }

    public bool HaveDishBlueberryIcecream()
    {
        return Item.Equals("DISH-BLUEBERRIES-ICE_CREAM");
    }

    public override string ToString()
    {
        return "Cooker \n\t" + Location.ToString() + " \n\tItem:" + Item;
    }
}
public class Table
{
    public Location Location { get; set; } = new Location();
    public string Item { get; set; }

    public override string ToString()
    {
        return "Table \n\t" + Location.ToString() + " \n\tItem:" + Item;
    }
}
public class Location
{
    public int X { get; set; }
    public int Y { get; set; }

    public override string ToString()
    {
        return "X :" + X + " Y:" + Y;
    }
}