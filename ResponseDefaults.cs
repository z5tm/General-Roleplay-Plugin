namespace GRPP;

using System.Text;
using EasyTmp;

public static class ResponseDefaults
{
    public static class ConsoleResponses
    {
        public static bool CannotBeRunByConsole(out string response)
        {
            response = EasyArgs.Build().Red("Error!").Space().Orange("This command cannot be run by the game console!").Done();
            return false;
        }
    }
    public static class PermissionResponses
    {
        public static bool NoPermission(out string response)
        {
            response = EasyArgs.Build().Red("Error!").Space().Orange("No permission!").Done();
            return false;
        }
        
        public static bool NoPermission(string permission, out string response)
        {
            response = EasyArgs.Build().Red("Error!").Space().Orange($"This requires the {permission} permission!").Done();
            return false;
        }
    }
    
    public static class InvalidOperationResponses
    {
        public static bool InvalidArgumentCount(out StringBuilder response)
        {
            response = new StringBuilder(EasyArgs.Build().Red("Error!").Space().Orange("Invalid argument count!").Done());
            return false;
        }
        
        public static bool InvalidArgumentCount(StringBuilder response)
        {
            response.Append(EasyArgs.Build().Red("Error!").Space().Orange("Invalid argument count!").Done());
            return false;
        }
        
        public static bool InvalidArgumentCount(out string response)
        {
            response = EasyArgs.Build().Red("Error!").Space().Orange("Invalid argument count!").Done();
            return false;
        }
        
        public static bool InvalidArgumentCount(bool overArgumentCount, out StringBuilder response)
        {
            response = new StringBuilder(EasyArgs.Build().Red("Error!").Space().Orange($"You are {(overArgumentCount ? "over" : "under")} the required argument count!").Done());
            return false;
        }
        
        public static bool InvalidArgumentCount(bool overArgumentCount, StringBuilder response)
        {
            response.Append(EasyArgs.Build().Red("Error!").Space().Orange($"You are {(overArgumentCount ? "over" : "under")} the required argument count!").Done());
            return false;
        }
        
        public static bool InvalidArgumentCount(bool overArgumentCount, out string response)
        {
            response = EasyArgs.Build().Red("Error!").Space().Orange($"You are {(overArgumentCount ? "over" : "under")} the required argument count!").Done();
            return false;
        }
        
        public static bool InvalidArgumentCount(int validArgumentCount, out StringBuilder response)
        {
            response = new StringBuilder(
                EasyArgs.Build()
                    .Red("Error!")
                    .Space().Orange("Invalid arguments count!")
                    .NewLine().Orange("Valid argument count:")
                    .Space().Blue(validArgumentCount.ToString()).Done()
            );
            return false;
        }
        
        public static bool InvalidArgumentCount(int validArgumentCount, StringBuilder response)
        {
            response.Append(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("Invalid arguments count!")
                .NewLine().Orange("Valid argument count:")
                .Space().Blue(validArgumentCount.ToString()).Done()
            );
            return false;
        }
        
        public static bool InvalidArgumentCount(int validArgumentCount, out string response)
        {
            response = EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("Invalid arguments count!")
                .NewLine().Orange("Valid argument count:")
                .Space().Blue(validArgumentCount.ToString()).Done();
            return false;
        }
        
        public static bool InvalidArgumentCount(bool overArgumentCount, int validArgumentCount, out StringBuilder response)
        {
            response = new StringBuilder(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange($"You are {(overArgumentCount ? "over" : "under")} the {(overArgumentCount ? "maximum" : "minimum")} argument count!")
                .NewLine().Orange("Valid argument count:")
                .Space().Blue(validArgumentCount.ToString()).Done());
            return false;
        }
        
        public static bool InvalidArgumentCount(bool overArgumentCount, int validArgumentCount, StringBuilder response)
        {
            response.Append(EasyArgs.Build()
                .Red("Error!")
                .Space().Orange($"You are {(overArgumentCount ? "over" : "under")} the {(overArgumentCount ? "maximum" : "minimum")} argument count!")
                .NewLine().Orange("Valid argument count:")
                .Space().Blue(validArgumentCount.ToString()).Done());
            return false;
        }
        
        public static bool InvalidArgumentCount(bool overArgumentCount, int validArgumentCount, out string response)
        {
            response = EasyArgs.Build()
                .Red("Error!")
                .Space().Orange($"You are {(overArgumentCount ? "over" : "under")} the {(overArgumentCount ? "maximum" : "minimum")} argument count!")
                .NewLine().Orange("Valid argument count:")
                .Space().Blue(validArgumentCount.ToString()).Done();
            return false;
        }
        
        public static bool InvalidArgumentCount(string commandUsage, int validArgumentCount, out string response)
        {
            response = EasyArgs.Build()
                .Red("Error!")
                .Space().Orange("Invalid arguments count!")
                .NewLine().Orange("Valid argument count:")
                .Space().Blue(validArgumentCount.ToString())
                .NewLine().Done() + commandUsage;
            return false;
        }
    }
}