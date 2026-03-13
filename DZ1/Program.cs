using System;

static int distance(string str1, string str2)
{
    if (str1 == null && str2 == null) { return 0; }

    int str1l = str1.Length;
    int str2l = str2.Length;

    if (str1l == 0) { return (str1l); }
    if (str2l == 0) { return (str2l); }

    str1 = str1.ToUpper();
    str2 = str2.ToUpper();

    int[,] matrix = new int[str1l+1,str2l+1];

    for (int i = 0; i < str1l; i++) { matrix[i, 0] = i; }
    for (int j = 0; j < str2l; j++) { matrix[0, j] = j; }

    for (int i = 1; i <= str1l; i++)
    {
        for (int j = 1; j <= str2l; j++)
        {
            int equal = (str1[i-1] == str2[j-1]) ? 0 : 1;
            int ins = matrix[i, j - 1] + 1;
            int del = matrix[i - 1, j] + 1;
            int subst = matrix[i - 1, j - 1] + equal;

            matrix[i, j] = Math.Min(Math.Min(ins, del), subst);

            if ((i > 1) && (j > 1) && (str1[i-1] == str2[j-2]) &&
                (str1[i-2] == str2[j-1]))
            {
                matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + equal);
            }

        }
    }
    return matrix[str1l - 1, str2l - 1];


 }
while (true)
{
    Console.Write("\nВведите первую строку (или 'exit' - выход): ");
    string s1 = Console.ReadLine();

    if (s1 == "exit") { break; }

    Console.Write("Введите вторую строку: ");
    string s2 = Console.ReadLine();

    int res = distance(s1, s2);

    Console.WriteLine("Расстояние Левенштейна: {0}", res);

}