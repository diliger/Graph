using Starcounter;

namespace Graph {
    partial class GraphsPage : Page {
        public void RefreshData() {
            Graphs = Db.SQL("SELECT i FROM Simplified.Ring6.Graph i");
        }

        // This attribute indicates which part of JSON tree 
        // is represented by this class
        [GraphsPage_json.Graphs]
        partial class GraphsItemPage {
            protected override void OnData() {
                base.OnData();
                this.Url = string.Format("/Graph/Details/{0}", this.Key);
            }
        }
    }
}
