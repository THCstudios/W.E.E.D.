using System;
using SharedMemory;
namespace Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{

			FeMoManager man = new FeMoManager();
			FeMoObject fem = new FeMoObject();
			fem.Id = 1;
			fem.Name = "Test";
			FeMoEntry fer = new FeMoEntry();
			fer.Name = "EntityType";
			fer.Value = "1";
			fer.Type = SharedMemory.Type.INT;
			fem.Manager = man;
			fem.AddEntry(fer);
			man.SendUpdateString();
			//TypenAPI api = new TypenAPI(fem);

			
			//FeMoManager man = new FeMoManager();
			//man.ReadFromFile("../../../initial_game.json", FeMoUpdateStringFormatter.JSON);
			////man.FileDump("dump.xml", FeMoUpdateStringFormatter.XML);
			////man.FileDump("sqlite.sql", FeMoUpdateStringFormatter.SQLITE);
			////man.FileDump("mysql.sql", FeMoUpdateStringFormatter.MYSQL);
			//man.SendUpdateString();
			////Console.WriteLine(man.Dump(FeMoUpdateStringFormatter.CONSOLE));
			//FeMoObject game = man.Get(0);
			//FeMoObject entities = game.GetObject("GameInfo").GetObject("Level").GetObject("Entities");
			//Global.DebugObject(entities);
			//int count = 0;
			//count = entities.GetInt("EntityCount");
			//for(int i = 0; i < count; i++) {
			//	FeMoObject entity = entities.GetObject("Entity_" + i);
			//	TypenAPI api = new TypenAPI(entity);
			//}
			
		}
	}
}
