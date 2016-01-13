using System;
using Starcounter;
using Simplified.Ring6;

namespace Graph {
    class Program {
        static void FillTestData() {
            long count = Db.SQL<long>("SELECT COUNT(g) FROM Simplified.Ring6.Graph g").First;
            if (count > 5) {
                return;
            }

            Db.Transact(() => {
                Simplified.Ring6.Graph gr = new Simplified.Ring6.Graph() { Name = "Graph" + (count + 1), Description = "Test graph " + (count + 1) };
                string[] names = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
                for (int i = 0; i < names.Length; i++) {
                    GraphValue v = new GraphValue() {
                        Graph = gr,
                        XValue = i + 1,
                        XLabel = names[i],
                        YValue = Math.Round(1.0M / ((i + 1) / (decimal)(names.Length / 2)), 4)
                    };
                }
            });
        }

        static void Main() {
            Handle.GET("/Graph", () => {
                MasterPage master;

                if (Session.Current != null && Session.Current.Data != null) {
                    master = (MasterPage)Session.Current.Data;
                } else {
                    master = new MasterPage();

                    if (Session.Current != null) {
                        master.Html = "/Graph/viewmodels/LauncherWrapperPage.html";
                        master.Session = Session.Current;
                    } else {
                        master.Html = "/Graph/viewmodels/MasterPage.html";
                        master.Session = new Session(SessionOptions.PatchVersioning);
                    }

                    master.RecentGraphs = new GraphsPage() {
                        Html = "/Graph/viewmodels/GraphsPage.html"
                    };
                }

                ((GraphsPage)master.RecentGraphs).RefreshData();
                master.FocusedGraph = null;

                return master;
            });

            //The bug! /Graph/Graphs/{?} returns Not found exception
            Handle.GET("/Graph/Details/{?}", (string Key) => {
                FillTestData();

                MasterPage master = Self.GET<MasterPage>("/Graph");
                master.FocusedGraph = Self.GET<GraphPage>("/Graph/Only/" + Key);
                return master;
            });

            Handle.GET("/Graph/Only/{?}", (string Key) => {
                GraphPage page = new GraphPage() {
                    Html = "/Graph/viewmodels/GraphPage.html",
                    Data = Db.SQL<Simplified.Ring6.Graph>(@"SELECT i FROM Simplified.Ring6.Graph i WHERE i.Key = ?", Key).First
                };

                return page;
            });

            Handle.GET("/Graph/menu", () => {
                return new Page() { Html = "/Graph/viewmodels/AppMenuPage.html" };
            });

            Handle.GET("/Graph/app-name", () => {
                return new AppName();
            });

            UriMapping.Map("/Graph/app-name", UriMapping.MappingUriPrefix + "/app-name");
            UriMapping.Map("/Graph/menu", UriMapping.MappingUriPrefix + "/menu");
        }
    }
}