using System;
using Starcounter;
using Simplified.Ring6;

namespace Graph {
    class Program {
        static void FillTestData() {
            long count = Db.SQL<long>("SELECT COUNT(g) FROM Simplified.Ring6.Graph g").First;
            if (count > 1) {
                return;
            }

            Db.Transact(() => {
                Simplified.Ring6.Graph gr = new Simplified.Ring6.Graph() { Name = "Graph" + (count + 1), Description = "Test graph " + (count + 1) };
                for (int i = 0; i < 10; i++) {
                    GraphValue v = new GraphValue() { Graph = gr, XValue = i, YValue = (decimal)Math.Pow(2, (double)i) };
                }
            });
        }

        static void Main() {
            FillTestData();

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
                MasterPage master = Self.GET<MasterPage>("/Graph");
                master.FocusedGraph = Db.Scope<GraphPage>(() => {
                    var page = new GraphPage() {
                        Html = "/Graph/viewmodels/GraphPage.html",
                        Data = Db.SQL<Simplified.Ring6.Graph>(@"SELECT i FROM Simplified.Ring6.Graph i WHERE i.Key = ?", Key).First
                    };

                    return page;
                });
                return master;
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