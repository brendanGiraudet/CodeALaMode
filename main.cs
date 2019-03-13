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
    public static Location ChoppedStrawBerryLocation { get; set; }

    static void Main(string[] args)
    {
        // customers list
        var customerList = GetCustomers().OrderByDescending(c => c.Award).ToList();

        //kitchen
        var kitchen = GetKitchen();

        // game loop
        while (true)
        {
            int turnsRemaining = int.Parse(Console.ReadLine());
            // me
            var me = GetCooker();
            // partner
            var partner = GetCooker();
            // table list
            kitchen.tables = GetTables();

            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            string ovenContents = inputs[0]; // ignore until wood 1 league
            int ovenTimer = int.Parse(inputs[1]);
            var customerWaitingList = GetCustomersWaiting().OrderByDescending(c => c.Award).ToList();

            // reponse
            string reponse = "";

            // récupération commande
            var order = customerWaitingList.First();
            // si chef a donner
            if (!me.HaveItem() && order.Completed)
            {
                customerList.Remove(order);
                order = customerList.First();
            }

            Console.Error.WriteLine(order);
            Console.Error.WriteLine(me);
            kitchen.tables.ForEach(t =>
            {
                Console.Error.WriteLine(t);
            });
            Console.Error.WriteLine(kitchen);


            /*
             * Si une commande a besoin de croissant
             * et je n'ai pas d'assiette de croissant dans les mains
             * et qu'il n'y a pas de table aillant un croissant
             */
            if (order.NeedCroissant() && !me.HaveDishCroissant() && kitchen.tables.Find(t => t.Item.Equals("CROISSANT")) == null && !me.HaveStrawberries() && !me.HaveChoppedStrawberries())
            {
                // déposer assiette si j'ai 
                if (me.HaveDish())
                {
                    var table = kitchen.GetAvailableTableLocationAroundDough();
                    if (table != null)
                    {
                        reponse = "USE " + table.X + " " + table.Y;
                    }

                }
                // deposer croissant a coté assiette
                if (me.HaveCroissant())
                {
                    var croissantTable = kitchen.GetAvailableTableLocationAroundDish();
                    if (croissantTable != null)
                    {
                        reponse = "USE " + croissantTable.X + " " + croissantTable.Y;
                    }
                }
                // si le four contient ma pate alors attendre
                else if (ovenContents.Equals("DOUGH"))
                {
                    reponse = "WAIT";
                }
                // si j'ai rien au four et que j'ai de la pate alors mettre au four
                else if ((ovenContents.Equals("NONE") && me.HaveDough()) || ovenContents.Equals("CROISSANT"))
                {
                    var oven = kitchen.GetOvenLocation();
                    if (oven != null)
                    {
                        reponse = "USE " + oven.X + " " + oven.Y;
                    }
                }
                // si je n'ai pas de pate alors aller chercher la pate
                else
                {
                    var dough = kitchen.GetDoughLocation();
                    if (dough != null)
                    {
                        reponse = "USE " + dough.X + " " + dough.Y;
                    }
                }
            }
            /*
             * si j'ai une commande de fraises coupées
             * et qu'il n'y a pas de table aillant de fraises coupées
             * et je n'ai pas d'assiette de fraises coupées dans les mains
            */
            else if (order.NeedChoppedStrawberries() && kitchen.tables.Find(t => t.Item.Equals("CHOPPED_STRAWBERRIES")) == null && !me.HaveDishChoppedStrawberries())
            {
                // déposer assiette si j'ai 
                if (me.HaveDish())
                {
                    var table = kitchen.GetAvailableTableLocationAroundStrawberries();
                    if (table != null)
                    {
                        reponse = "USE " + table.X + " " + table.Y;
                    }

                }
                // deposer fraises coupées a coté assiette
                if (me.HaveChoppedStrawberries())
                {
                    var table = kitchen.GetAvailableTableLocationAroundDish();
                    if (table != null)
                    {
                        reponse = "USE " + table.X + " " + table.Y;
                    }
                }
                // rechercher planche à découper
                else if (me.HaveStrawberries())
                {
                    var choppedBoard = kitchen.GetChoppingBoardLocation();
                    if (choppedBoard != null)
                    {
                        reponse = "USE " + choppedBoard.X + " " + choppedBoard.Y;
                    }
                }
                // rechercher les fraises
                else
                {
                    var strawberries = kitchen.GetStrawberriesLocation();
                    if (strawberries != null)
                    {
                        reponse = "USE " + strawberries.X + " " + strawberries.Y;
                    }
                }
            }
            // si j'ai quelque chose dans les mains
            else if (me.HaveItem())
            {
                //locate window
                if (me.HaveCompletedOrder(order))
                {
                    var window = kitchen.GetWindowLocation();
                    if (window != null)
                    {
                        reponse = "USE " + window.X + " " + window.Y;
                        order.Completed = true;
                    }
                }

                // si commande pas fini alors aller chercher élément
                if (string.IsNullOrEmpty(reponse))
                {
                    // récupération des produits à faire
                    var orderedItem = order.Item.Split('-').ToList();
                    var item = orderedItem.Where(i => !me.Item.Contains(i) && !i.Equals("DISH")).FirstOrDefault();
                    if (item != null)
                    {
                        switch (item)
                        {
                            // locate icecream
                            case "ICE_CREAM":
                                var icecream = kitchen.GetIcecreamLocation();
                                if (icecream != null)
                                {
                                    reponse = "USE " + icecream.X + " " + icecream.Y;
                                }
                                break;
                            // locate blueberries
                            case "BLUEBERRIES":
                                var blueberry = kitchen.GetBlueberriesLocation();
                                if (blueberry != null)
                                {
                                    reponse = "USE " + blueberry.X + " " + blueberry.Y;
                                }
                                break;
                            // locate chopped strawberries
                            case "CHOPPED_STRAWBERRIES":
                                var choppedStrawberriesTable = kitchen.tables.Find(t => t.Item.Equals("CHOPPED_STRAWBERRIES"));
                                if (choppedStrawberriesTable != null)
                                {
                                    reponse = "USE " + choppedStrawberriesTable.Location.X + " " + choppedStrawberriesTable.Location.Y;
                                }
                                break;
                            // locate croissant
                            case "CROISSANT":
                                var croissantTable = kitchen.tables.Find(t => t.Item.Equals("CROISSANT"));
                                if (croissantTable != null)
                                {
                                    reponse = "USE " + croissantTable.Location.X + " " + croissantTable.Location.Y;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                // locate dish dropped
                var dishDropped = kitchen.tables.Find(t => t.Item.Equals("DISH"));
                if (dishDropped != null)
                {
                    reponse = "USE " + dishDropped.Location.X + " " + dishDropped.Location.Y;
                }
                else
                {
                    // dish washer 
                    var dish = kitchen.GetDishLocation();
                    if (dish != null)
                    {
                        reponse = "USE " + dish.X + " " + dish.Y;
                    }
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
    private static List<Customer> GetCustomersWaiting()
    {
        var list = new List<Customer>();
        int numCustomers = int.Parse(Console.ReadLine()); // the number of customers currently waiting for food
        for (int i = 0; i < numCustomers; i++)
        {
            var inputs = Console.ReadLine().Split(' ');
            list.Add(new Customer
            {
                Item = inputs[0],
                Award = int.Parse(inputs[1])
            });
        }
        return list;
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
    public List<Table> tables { get; set; } = new List<Table>();

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

    public Location GetDoughLocation()
    {
        for (int i = 0; i < Lines.Count(); i++)
        {
            for (int j = 0; j < Lines[i].Count(); j++)
            {
                if (Lines[i][j].Equals('H'))
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

    public Location GetOvenLocation()
    {
        for (int i = 0; i < Lines.Count(); i++)
        {
            for (int j = 0; j < Lines[i].Count(); j++)
            {
                if (Lines[i][j].Equals('O'))
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

    public Location GetAvailableTableLocationAroundStrawberries()
    {
        var dish = GetStrawberriesLocation();
        // table de gauche
        if (Lines[dish.Y][dish.X - 1].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X - 1) && t.Location.Y.Equals(dish.Y)) == null)
        {
            dish.X--;
            return dish;
        }
        // table de droite
        if (Lines[dish.Y][dish.X + 1].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X + 1) && t.Location.Y.Equals(dish.Y)) == null)
        {
            dish.X++;
            return dish;
        }
        // table du haut
        if (Lines[dish.Y - 1][dish.X].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X) && t.Location.Y.Equals(dish.Y - 1)) == null)
        {
            dish.Y--;
            return dish;
        }
        // table du bas
        if (Lines[dish.Y + 1][dish.X].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X) && t.Location.Y.Equals(dish.Y + 1)) == null)
        {
            dish.Y++;
            return dish;
        }
        return null;
    }
    public Location GetAvailableTableLocationAroundDough()
    {
        var dish = GetDoughLocation();
        // table de gauche
        if (Lines[dish.Y][dish.X - 1].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X - 1) && t.Location.Y.Equals(dish.Y)) == null)
        {
            dish.X--;
            return dish;
        }
        // table de droite
        if (Lines[dish.Y][dish.X + 1].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X + 1) && t.Location.Y.Equals(dish.Y)) == null)
        {
            dish.X++;
            return dish;
        }
        // table du haut
        if (Lines[dish.Y - 1][dish.X].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X) && t.Location.Y.Equals(dish.Y - 1)) == null)
        {
            dish.Y--;
            return dish;
        }
        // table du bas
        if (Lines[dish.Y + 1][dish.X].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X) && t.Location.Y.Equals(dish.Y + 1)) == null)
        {
            dish.Y++;
            return dish;
        }
        return null;
    }

    public Location GetAvailableTableLocationAroundDish()
    {
        var dish = GetDishLocation();
        // table de gauche
        if (Lines[dish.Y][dish.X - 1].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X - 1) && t.Location.Y.Equals(dish.Y)) == null)
        {
            dish.X--;
            return dish;
        }
        // table de droite
        if (Lines[dish.Y][dish.X + 1].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X + 1) && t.Location.Y.Equals(dish.Y)) == null)
        {
            dish.X++;
            return dish;
        }
        // table du haut
        if (Lines[dish.Y - 1][dish.X].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X) && t.Location.Y.Equals(dish.Y - 1)) == null)
        {
            dish.Y--;
            return dish;
        }
        // table du bas
        if (Lines[dish.Y + 1][dish.X].Equals('#') && tables.Find(t => t.Location.X.Equals(dish.X) && t.Location.Y.Equals(dish.Y + 1)) == null)
        {
            dish.Y++;
            return dish;
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

    public Location GetChoppingBoardLocation()
    {
        for (int i = 0; i < Lines.Count(); i++)
        {
            for (int j = 0; j < Lines[i].Count(); j++)
            {
                if (Lines[i][j].Equals('C'))
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

    public Location GetStrawberriesLocation()
    {
        for (int i = 0; i < Lines.Count(); i++)
        {
            for (int j = 0; j < Lines[i].Count(); j++)
            {
                if (Lines[i][j].Equals('S'))
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
    public bool Completed { get; set; } = false;

    public bool NeedChoppedStrawberries()
    {
        return Item.Split('-').ToList().Contains("CHOPPED_STRAWBERRIES");
    }

    public bool NeedCroissant()
    {
        return Item.Split('-').ToList().Contains("CROISSANT");
    }

    public bool NeedBlueberries()
    {
        return Item.Split('-').ToList().Contains("BLUEBERRIES");
    }

    public bool NeedIcecream()
    {
        return Item.Split('-').ToList().Contains("ICE_CREAM");
    }

    public override string ToString()
    {
        return "Customer \n\taward :" + this.Award + "\t item:" + this.Item;
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
        return (!string.IsNullOrEmpty(Item) && !Item.Equals("NONE"));
    }

    public bool HaveDishBlueberry()
    {
        return Item.Split('-').ToList().Contains("BLUEBERRIES") && Item.Split('-').ToList().Contains("DISH");
    }

    public bool HaveDishCroissant()
    {
        return Item.Split('-').ToList().Contains("CROISSANT") && Item.Split('-').ToList().Contains("DISH");
    }

    public bool HaveCroissant()
    {
        return Item.Equals("CROISSANT");
    }

    public bool HaveDough()
    {
        return Item.Equals("DOUGH");
    }

    public bool HaveDishIcecream()
    {
        return Item.Split('-').ToList().Contains("ICE_CREAM") && Item.Split('-').ToList().Contains("DISH"); ;
    }

    public bool HaveDishBlueberryIcecream()
    {
        return Item.Equals("DISH-BLUEBERRIES-ICE_CREAM");
    }

    public bool HaveStrawberries()
    {
        return Item.Equals("STRAWBERRIES");
    }

    public bool HaveDishChoppedStrawberries()
    {
        return Item.Split('-').ToList().Contains("CHOPPED_STRAWBERRIES") && Item.Split('-').ToList().Contains("DISH"); ;
    }

    public bool HaveChoppedStrawberries()
    {
        return Item.Equals("CHOPPED_STRAWBERRIES");
    }

    public bool HaveCompletedOrder(Customer order)
    {
        return Item.Equals(order.Item);
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

    public bool HaveChoppedStrawberries()
    {
        return Item.Equals("CHOPPED_STRAWBERRIES");
    }

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