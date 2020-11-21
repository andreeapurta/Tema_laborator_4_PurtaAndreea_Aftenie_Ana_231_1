using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Tema_laborator_4_PurtaAndreea_Aftenie_Ana_231_1
{

    class Program
    {
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        //primeste ca parametrii sirul si t1,t2,t3,...indicele
        public static void Emit(string sir, int count)
        {
            using (StreamWriter fisierout = new StreamWriter(@"C:\Users\Andreea Purta\source\repos\Tema_laborator_4_PurtaAndreea_Aftenie_Ana_231_1\Tema_laborator_4_PurtaAndreea_Aftenie_Ana_231_1\outFile.txt", true))
            {
                fisierout.WriteLine("t" + count + "=" + sir);
            }
        }

        static void Main(string[] args)
        {
            Stack stack = new Stack();
            Stack attributeStrack = new Stack();
            string terminals = "";
            string nonterminals = "";
            var production = new List<(string, string)>();
            string[,] TS = new string[100, 100];
            string[,] TA = new string[100, 100];
            string cuvIntrare = "";
            int productionLength;
            int linesTA = 0, colTA = 0, linesTS = 0, colTS = 0;
            int indexLine = 1;
            int indexCol = 0;
            string value = "";
            string tempTerm = "";
            int countForTs = 1;
            List<int> complexProd = new List<int>();
            List<int> finalProd = new List<int>();
            //citire fisier
            using (StreamReader fisier = new StreamReader(@"C:\Users\Andreea Purta\source\repos\Tema_laborator_4_PurtaAndreea_Aftenie_Ana_231_1\Tema_laborator_4_PurtaAndreea_Aftenie_Ana_231_1\TextFile.txt"))
            {
                nonterminals += fisier.ReadLine();
                terminals += fisier.ReadLine();
                Int32.TryParse(fisier.ReadLine(), out productionLength);
                for (int i = 0; i < productionLength; i++)
                {
                    string[] sirImpartit = fisier.ReadLine().Split(" ");
                    production.Add((sirImpartit[0], sirImpartit[1]));

                    //Pt a afla care sunt productiile complexe
                    int ct = 0;
                    //sir impartit[1] = ceea ce am in partea dreapta, daca in partea dreapta am 2 neterminale atunci avem productie complexa
                    for (int j = 0; j < nonterminals.Length; j++)
                    {
                        if (sirImpartit[1].Contains(nonterminals[j]))
                        {
                            ct++;
                            if (ct == 2)
                            {
                                complexProd.Add(i + 1);
                            }
                        }
                    }
                    //aici aflu care sunt productiile care genereaza neterminal a pt ca astea se baga in stiva si nu se mai da pop la nimic.
                    if (sirImpartit[1].Contains('a'))
                    {
                        finalProd.Add(i + 1);
                    }
                }

                Int32.TryParse(fisier.ReadLine(), out linesTA);
                Int32.TryParse(fisier.ReadLine(), out colTA);

                for (int i = 0; i < linesTA; i++)
                {
                    string line = fisier.ReadLine();
                    string[] terms = line.Split(" ");
                    for (int j = 0; j < colTA; j++)
                    {
                        TA[i, j] = terms[j];
                    }
                }

                Int32.TryParse(fisier.ReadLine(), out linesTS);
                Int32.TryParse(fisier.ReadLine(), out colTS);

                for (int i = 0; i < linesTS; i++)
                {
                    string lineTS = fisier.ReadLine();
                    string[] termsTS = lineTS.Split(" ");
                    for (int j = 0; j < colTS; j++)
                    {
                        TS[i, j] = termsTS[j];
                    }
                }
                cuvIntrare += fisier.ReadLine();
            }

            //afisari
            Console.WriteLine(nonterminals);
            Console.WriteLine(terminals);
            Console.WriteLine("Amu afisam matricea TA");
            for (int i = 0; i < linesTA; i++)
            {
                for (int j = 0; j < colTA; j++)
                {
                    Console.Write(TA[i, j] + "\t");

                }
                Console.WriteLine();
            }

            Console.WriteLine("Amu afisam matricea TS");
            for (int i = 0; i < linesTS; i++)
            {
                for (int j = 0; j < colTS; j++)
                {
                    Console.Write(TS[i, j] + "\t");

                }
                Console.WriteLine();
            }
            Console.WriteLine("Amu afisam cuvant intrari " + cuvIntrare);

            //initializare stiva cu $ si 0
            stack.Push('$');
            stack.Push(0);

            bool acc = false;
            while (!acc)
            {

                ///daca e int
                if (stack.Peek().GetType() == typeof(int))
                {
                    indexLine = (int)(stack.Peek());
                    indexCol = terminals.IndexOf(cuvIntrare[0]);
                    value = TA[indexLine, indexCol];

                    if (value.Equals("acc"))
                    {
                        Console.WriteLine("Amu apartine gramaticii!! :)");
                        acc = true;
                    }

                    if (value.Equals("0"))
                    {
                        Console.WriteLine("Amu Nu apartine gramaticii! :(");
                        return;
                    }

                    if (value[0] == 'd')
                    {
                        stack.Push(cuvIntrare[0]);
                        stack.Push(int.Parse(value.Substring(1)));
                        cuvIntrare = cuvIntrare.Substring(1, cuvIntrare.Length - 1);
                    }

                    else if (value[0] == 'r')
                    {
                        string auxStack = "";
                        //parcurgem stiva (Care am facut o array)
                        foreach (var item in stack.ToArray())
                        {
                            //salvez semnul dintre o productie complexa  //am verificat daca in stiva avem litera si e terminala adica +*...
                            if ((stack.Peek().GetType() == typeof(char) || item.GetType() == typeof(string)) && terminals.Contains(stack.Peek().ToString()))
                            {
                                tempTerm = stack.Peek().ToString();
                            }

                            stack.Pop();

                            //daca in stiva e caracter atunci adaug in sir
                            if (item.GetType() == typeof(char) || item.GetType() == typeof(string))
                            {
                                auxStack += item;
                                var altavar = production[int.Parse(value[1].ToString()) - 1].Item2;
                                //daca am gasit in stiva elementul de la care se face reducere se opreste daca nu tot scoate din stiva
                                if (Reverse(auxStack).Equals(altavar))
                                {
                                    break;
                                }
                            }
                        }
                        //pune in stiva doar daca este productie finala adica cu terminal in capat  adica daca nu e complexa dau push
                        //de cate ori e cuvant de intrare
                        if (finalProd.Contains(int.Parse(value[1].ToString())))
                        {
                            attributeStrack.Push('a');
                        }

                        //daca e dubla, atunci trebe sa punem in striva 
                        //scoatem in t1 si t2 ultimele 2 chestii din stiva 
                        if (complexProd.Contains(int.Parse(value[1].ToString())))
                        {
                            var t1 = attributeStrack.Pop();
                            var t2 = attributeStrack.Pop();
                            var temp = t2 + tempTerm + t1;
                            //scriu in fisier linia de cod generata 
                            Emit(temp, countForTs);
                            //pun pe stiva termenul anterior adaugat
                            attributeStrack.Push("t" + countForTs);
                            countForTs++;
                        }

                        var peek = (int)(stack.Peek());
                        stack.Push(production[int.Parse(value[1].ToString()) - 1].Item1);
                        var auxVar = nonterminals.IndexOf(stack.Peek().ToString());
                        stack.Push(int.Parse(TS[peek, auxVar]));
                    }
                }
                //daca e string
                else
                {
                    //cauta in tabela de salt urmatoarea actiune
                    var auxCol = nonterminals.IndexOf((char)stack.Pop());
                    int action = Int32.Parse(TS[(int)stack.Peek(), auxCol]);
                    //si pune pe stiva
                    stack.Push(nonterminals[auxCol]);
                    stack.Push(action);
                }

            }
        }

    }
}

