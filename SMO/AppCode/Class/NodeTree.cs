using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public class Node
    {
        public string id { get; set; }
        public string pId { get; set; }
        public string name { get; set; }
        public string @checked { get; set; }
        public string icon { get; set; }
        public string open { get; set; }
        public string companyCode { get; set; }
        public Node()
        {
            @checked = "false";
            open = "true";
        }
    }

    public class NodeOrganize : Node
    {
        public NodeOrganize() : base()
        {

        }
    }

    public class NodeConfig : Node
    {
        public string companyCode { get; set; }
        public string modulType { get; set; }
        public NodeConfig() : base()
        {

        }
    }

    public class NodeUser : Node
    {
        public string userName { get; set; }
        public string type { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public NodeUser() : base()
        {

        }
    }

    public class NodeRight : Node
    {
        public string isAdd { get; set; }
        public string isRemove { get; set; }
        public NodeRight() : base()
        {

        }
    }
    public class NodeMenu : Node
    {
        public NodeMenu() : base()
        {

        }
    }

    public class NodeWorkFlow : Node
    {
        public string type { get; set; }
        public NodeWorkFlow() : base()
        {

        }
    }
}