using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security;
using System.Threading;

namespace Warcaby_Server
{
    public static class DatabaseManager
    {

        private static SqlConnection _sqlConnection;

        public static void OpenSqlConnection()
        {
            _sqlConnection = new SqlConnection(GIT_IGNORED_FILE.DBconnectionString);
            _sqlConnection.Open();

            CreateAccountTableIfItDoesNotExist();
            CreatePlayerDataTableIfItDoesNotExist();
            CreateSkinOffersTableIfItDoesNotExist();
            CreateInventoriesTableIfItDoesNotExist();
        }

        public static void CloseSqlConnection()
        {
            _sqlConnection.Close();
        }

        public static bool AccountCredentialsCorrect(string accountName, string accountPassword)
        {
            string cmdText = "SELECT COUNT (*) FROM [dbo].[Accounts] WHERE name = @accountName AND password = @accountPassword";

            var command = new SqlCommand(cmdText, _sqlConnection);
            command.Parameters.AddWithValue("@accountName", accountName);
            command.Parameters.AddWithValue("@accountPassword", accountPassword);
            var numberOfAccounts = command.ExecuteScalar();

            return Convert.ToInt32(numberOfAccounts) == 1;
        } 
        
        public static void GetPlayerDataFromDatabaseAndInsertInto(ref PlayerData playerData)
        {
            string cmdText = "SELECT * FROM [dbo].[PlayerData] INNER JOIN [dbo].[Accounts] ON PlayerData.accountId = Accounts.id WHERE Accounts.name = @accountName";

            var command = new SqlCommand(cmdText, _sqlConnection);
            command.Parameters.AddWithValue("@accountName", playerData.AccountName);
            
            var reader = command.ExecuteReader();
            reader.Read();
            playerData.Coins = reader.GetInt32(2);

            playerData.LastLoginDate = reader.GetDateTime(3);

            playerData.SelectedSkinsIdsForPawns = new int[12];
            for(var i=0; i<playerData.SelectedSkinsIdsForPawns.Length; i++)
                playerData.SelectedSkinsIdsForPawns[i] = reader.GetInt32(i+4);
            
            reader.Close();

            playerData.OwnedSkinIds = GetPlayerOwnedSkinIds(playerData.AccountName);
        }

        private static List<int> GetPlayerOwnedSkinIds(string accountName)
        {
            string cmdText = "SELECT skinId FROM [dbo].[Inventories] INNER JOIN [dbo].[Accounts] ON [dbo].[Inventories].accountId = [dbo].[Accounts].id WHERE Accounts.name = @accountName";
        
            var command = new SqlCommand(cmdText, _sqlConnection);
            command.Parameters.AddWithValue("@accountName", accountName);

            var playerOwnedSkinsIds = new List<int>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    playerOwnedSkinsIds.Add(reader.GetInt32(0));
            }

            return playerOwnedSkinsIds;
        }
        
        public static void UpdatePlayerDataInDatabase(PlayerData playerData)
        {
            string cmdText = "UPDATE [dbo].[PlayerData] SET " + 
                             "[dbo].[PlayerData].coins = @coinsValue, " +
                             "[dbo].[PlayerData].lastLoginDate = @lastLoginDateValue, " +
                             "[dbo].[PlayerData].pawnSkin1 = @pawnSkin1Value, " + 
                             "[dbo].[PlayerData].pawnSkin2 = @pawnSkin2Value, " +
                             "[dbo].[PlayerData].pawnSkin3 = @pawnSkin3Value, " +
                             "[dbo].[PlayerData].pawnSkin4 = @pawnSkin4Value, " +
                             "[dbo].[PlayerData].pawnSkin5 = @pawnSkin5Value, " +
                             "[dbo].[PlayerData].pawnSkin6 = @pawnSkin6Value, " +
                             "[dbo].[PlayerData].pawnSkin7 = @pawnSkin7Value, " +
                             "[dbo].[PlayerData].pawnSkin8 = @pawnSkin8Value, " +
                             "[dbo].[PlayerData].pawnSkin9 = @pawnSkin9Value, " +
                             "[dbo].[PlayerData].pawnSkin10 = @pawnSkin10Value, " +
                             "[dbo].[PlayerData].pawnSkin11 = @pawnSkin11Value, " +
                             "[dbo].[PlayerData].pawnSkin12 = @pawnSkin12Value " +
                             "FROM [dbo].[PlayerData] INNER JOIN [dbo].[Accounts] ON PlayerData.accountId = Accounts.id " + 
                             "WHERE Accounts.name = @accountName";

            var command = new SqlCommand(cmdText, _sqlConnection);
            command.Parameters.AddWithValue("@coinsValue", playerData.Coins);
            command.Parameters.AddWithValue("@lastLoginDateValue", playerData.LastLoginDate);

            for (var i = 0; i < 12; i++)
                command.Parameters.AddWithValue("@pawnSkin" + (i+1).ToString() + "Value", playerData.SelectedSkinsIdsForPawns[i]);

            command.Parameters.AddWithValue("@accountName", playerData.AccountName);
            
            command.ExecuteNonQuery();

            DeleteInventoryAndFillItWithNewValues(playerData);
        }

        private static void DeleteInventoryAndFillItWithNewValues(PlayerData playerData)
        {
            string accountIdSelectCmdText = "SELECT id FROM [dbo].[Accounts] WHERE name = @accountName";
            var accountIdSelectCommand = new SqlCommand(accountIdSelectCmdText, _sqlConnection);
            accountIdSelectCommand.Parameters.AddWithValue("@accountName", playerData.AccountName);
            int accountId = (int) accountIdSelectCommand.ExecuteScalar();
            
            
            string deleteInvCmdText = "DELETE FROM [dbo].[Inventories] WHERE [dbo].[Inventories].accountId = @accountId";
            var deleteInvCommand = new SqlCommand(deleteInvCmdText, _sqlConnection);
            deleteInvCommand.Parameters.AddWithValue("@accountId", accountId);
            deleteInvCommand.ExecuteNonQuery();


            foreach (var skinId in playerData.OwnedSkinIds)
            {
                string insertSkinCmdText = "INSERT INTO [dbo].[Inventories] (accountId, skinId) VALUES (@accountId, @skinId)";
                var insertSkinCommand = new SqlCommand(insertSkinCmdText, _sqlConnection);
                insertSkinCommand.Parameters.AddWithValue("@accountId", accountId);
                insertSkinCommand.Parameters.AddWithValue("@skinId", skinId);
                insertSkinCommand.ExecuteNonQuery();
            }
        }

        public static SkinOffer[] GetSkinOffersFromDatabase()
        {
            string cmdText = "SELECT * FROM [dbo].[SkinOffers]";
        
            var command = new SqlCommand(cmdText, _sqlConnection);

            var skinOffers = new List<SkinOffer>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    SkinOffer skinOffer;
                    skinOffer.Id = reader.GetInt32(0);
                    skinOffer.Price = reader.GetInt32(1);
                    
                    skinOffers.Add(skinOffer);
                }
            }

            return skinOffers.ToArray();
        }

        private static void CreateAccountTableIfItDoesNotExist()
        {
            string cmdText = 
                "IF object_id('Accounts', 'U') is null" + 
	            "    CREATE TABLE Accounts (" +
		        "        [id] int not null identity," +
		        "        [name] nvarchar(255)," +
		        "        [password] nvarchar(255)," +
                "        PRIMARY KEY( [id] )" + 
	            "    );";

            var command = new SqlCommand(cmdText, _sqlConnection);
            command.ExecuteNonQuery();
        }
        
        private static void CreatePlayerDataTableIfItDoesNotExist()
        {
            string cmdText = 
                "IF object_id('PlayerData', 'U') is null" + 
                "    CREATE TABLE PlayerData (" +
                "        [id] int not null identity," +
                "        [accountId] int not null," +
                "        [coins] int DEFAULT 0," +
                "        [lastLoginDate] date DEFAULT '2000-01-01'," +
                "        [pawnSkin1] int DEFAULT 0," +
                "        [pawnSkin2] int DEFAULT 0," +
                "        [pawnSkin3] int DEFAULT 0," +
                "        [pawnSkin4] int DEFAULT 0," +
                "        [pawnSkin5] int DEFAULT 0," +
                "        [pawnSkin6] int DEFAULT 0," +
                "        [pawnSkin7] int DEFAULT 0," +
                "        [pawnSkin8] int DEFAULT 0," +
                "        [pawnSkin9] int DEFAULT 0," +
                "        [pawnSkin10] int DEFAULT 0," +
                "        [pawnSkin11] int DEFAULT 0," +
                "        [pawnSkin12] int DEFAULT 0," +
                "        PRIMARY KEY( [id] )" + 
                "    );";

            var command = new SqlCommand(cmdText, _sqlConnection);
            command.ExecuteNonQuery();
        }
        
        private static void CreateSkinOffersTableIfItDoesNotExist()
        {
            string cmdText = 
                "IF object_id('SkinOffers', 'U') is null" + 
                "    CREATE TABLE SkinOffers (" +
                "        [id] int not null identity," +
                "        [price] int not null," +
                "        PRIMARY KEY( [id] )" + 
                "    );";

            var command = new SqlCommand(cmdText, _sqlConnection);
            command.ExecuteNonQuery();
        }
        
        private static void CreateInventoriesTableIfItDoesNotExist()
        {
            string cmdText = 
                "IF object_id('Inventories', 'U') is null" + 
                "    CREATE TABLE Inventories (" +
                "        [accountId] int not null," +
                "        [skinId] int not null" +
                "    );";

            var command = new SqlCommand(cmdText, _sqlConnection);
            command.ExecuteNonQuery();
        }
    }
}