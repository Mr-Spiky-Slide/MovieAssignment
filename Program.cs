using NLog;
using System;
using System.Text.RegularExpressions;

string path = Directory.GetCurrentDirectory() + "/nlog.config";
// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();

// See https://aka.ms/new-console-template for more information

string file = "ml-latest-small/movies.csv";

// ask for input
Console.WriteLine("Enter 1 to list movies.");
Console.WriteLine("Enter 2 to enter data.");
Console.WriteLine("Enter anything else to quit.");
// input response
string resp = Console.ReadLine();
if (resp == "1")
{
    StreamReader sr = new StreamReader(file);
        while (!sr.EndOfStream)
        {

            Console.WriteLine();
            logger.Trace("Itterating over line");
            Console.WriteLine();
            string? currentLine = sr.ReadLine();
            if (currentLine is null)
            {
                logger.Warn("Null Line");//add a way to count lines
                continue;
            }

            
            Regex rx = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            string[] lineArray = rx.Split(currentLine);

            string[] currentGenres = lineArray[2].Split("|");

            string genre = currentGenres.Length == 1 ? "Genre: " : "Genres: ";
            foreach (string genreType in currentGenres)
            {
                genre += $"{genreType} ";
            }
            lineArray[1] = lineArray[1].Replace("\"", "");

            Console.Write($"-> {lineArray[1]} \n{genre}");


        }


        sr.Close();
    
   
}

else if (resp == "2")
{
    //read data from file
    if (File.Exists(file))
    {
        //stream READER
        StreamWriter sw = new StreamWriter(file, append: true);
        StreamReader sr = new StreamReader(file);
        List<string> allMovies = new List<string>();

        //loops tru each line of the file
        while(!sr.EndOfStream)
        {
            string? currentLine = sr.ReadLine();
            Regex rx = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            string[] lineArray = rx.Split(currentLine);

            lineArray[1] = lineArray[1].Replace("\"", "");
            allMovies.Add(lineArray[1]);
        }
        string newMovieGenres;
        string newMovieTitle;
        bool keepAdding = true;
        do{
            Console.WriteLine("What is the Title of the movie you would like to add?");
            newMovieTitle = Console.ReadLine();

            int? movieExist = allMovies.IndexOf(newMovieTitle);
            if(movieExist < 0){

                if(newMovieTitle.Contains(",")){
                    newMovieTitle = newMovieTitle.Insert(0,"\"");
                    newMovieTitle += "\"";
                }

                Console.WriteLine("Enter a genre: ");
                newMovieGenres = Console.ReadLine();
                bool anotherGenre;
                Console.WriteLine("Do you want to add another genre? (y/N)");
                string genreInput = Console.ReadLine();
                if(genreInput[0] == 'y'){
                    anotherGenre = true;
                }
                else{
                    anotherGenre = false;
                }
                while(anotherGenre){
                    Console.WriteLine("Enter a genre: ");
                    newMovieGenres += "|"+Console.ReadLine();
                    Console.WriteLine("Do you want to add another genre? (y/N)");
                    genreInput = Console.ReadLine();
                    if(genreInput[0] == 'y'){
                        anotherGenre = true;
                    }
                    else{
                        anotherGenre = false;
                    }
                }
                
                string lastLine = File.ReadLines(file).LastOrDefault();
                string[] lastArray = lastLine.Split(",");
                int lastId = Convert.ToInt32(lastArray[0]);

                sw.WriteLine($"{allMovies.Count()+1},{newMovieTitle},{newMovieGenres}");
                //Console.WriteLine($"{lastId+1},{newMovieTitle},{newMovieGenres}");

            }
            else{
                logger.Warn("Movie already exists in database");
                
            }
            Console.WriteLine("Do you want to add another movie? (y/N)");
            string input = Console.ReadLine();
            if(input[0] == 'y'){
                keepAdding = true;
            }
            else{
                keepAdding = false;
            }
        }while(keepAdding);

        try{

        }
        catch (Exception ex){
            logger.Error(ex.Message);
        }
        

    }
    else{
        Console.WriteLine("file does not exist");
    }


}



