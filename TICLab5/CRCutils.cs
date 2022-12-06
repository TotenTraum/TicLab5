using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TICLab5;

public static class CrcUtils
{
    // Поиск r
    public static int CalculateCodeDistance(int count) => (int)Math.Ceiling(Math.Log2((count + 1) + Math.Ceiling(Math.Log2(count + 1))));

    // Подсчёт символов
    public static int CalculateDigitsCount(BigInteger value)
    {
        var tmp = value;
        var r = 0;
        while (tmp > 0)
        {
            tmp /= 10;
            r++;
        }
        return r;
    }

// Поиск единиц в числе
    public static int CountOne(BigInteger value)
    {
        var tmp = value;
        var r = 0;
        while (tmp > 0)
        {
            if(tmp % 10 == 1)
                r++;
            tmp /= 10;
        }
        return r;
    }
    
    // Находит числа больше 1 и переводит их в след разряд
    public static void FixBinaryInput(ref BigInteger value)
    {
        var digits = BigIntToList(value);
        for (int i = 0; i < digits.Count; i++)
        {
            if (digits[i] <= 1) continue;
            if (i + 1 == digits.Count)
                digits.Add(1);
            else
                digits[i + 1] += 1;
            digits[i] = 0;
        }
        value = ListToBigInt(digits);
    }

    // Перевод из big int в спискок
    public static List<int> BigIntToList(BigInteger value)
    {
        var digits = new List<int>();
        while (value > 0)
        {
            digits.Add((int)(value % 10));
            value /= 10;
        }
        return digits;
    }

    // Перевод из списка в big int с потерей нулей
    public static BigInteger ListToBigInt(List<int> digits)
    {
        var newValue = new BigInteger(0);
        for (int i = digits.Count - 1; i >= 0; i--)
            newValue = newValue* 10 + digits[i];
        return newValue;
    }

    // Вычленяет countDigits символов
    public static BigInteger ListToBigIntCount(ref List<int> digits, int countDigits)
    {
        var newValue = new BigInteger(0);
        int i = digits.Count - 1, count = 0;
        for (; count != countDigits && digits.Count != 0; i--, count++)
        {
            newValue = newValue* 10 + digits[i];
            digits.RemoveAt(digits.Count - 1);
        }
        return newValue;
    }

    // Очистка лишних нулей
    public static List<int> TrimList(List<int> value)
    {
        int index = value.Count - 1;
        for (; index > 0 && value[index] == 0; index--)
            value.RemoveAt(index);
        return value;
    }

    // Деление код. комбинации на код. комбинацию
    public static List<int> DivLists(List<int> first, List<int> second)
    {
        first = TrimList(first);
        second = TrimList(second);

        for (int i = 0; i < second.Count; i++)
            if (first[second.Count - 1 - i] == 1 && second[second.Count - 1 - i] == 1)
                first[second.Count - 1 - i] = 0;
            else if (first[second.Count - 1 - i] == 1 && second[second.Count - 1 - i] == 0)
                first[second.Count - 1 - i] = 1;
            else if (first[second.Count - 1 - i] == 0 && second[second.Count - 1 - i] == 1)
                first[second.Count - i - 1] = 1;
        return first;
    }

    public static List<int> AddLists(List<int> first, List<int> second)
    {
        var firstInt = ListToBigInt(first);
        var secondInt = ListToBigInt(second);
        
        if (firstInt < secondInt)
            (first, second) = (second, first);
    
        var sizeMax = first.Count;
        
        for (int i = 0; i < second.Count; i++)
            if (first[i] == 1 && second[i] == 1)
            {
                first[i] = 0;
                if (i + 1 == first.Count)
                    first.Add(1);
                else
                    first[i + 1] += 1;
                var tmp = ListToBigInt(first);
                FixBinaryInput(ref tmp);
                first = BigIntToList(tmp);
            }
            else
                first[i] += second[i];
    
        while (first.Count < sizeMax)
        {
            first.Add(0);
        }
        
        return first;
    }


    // xor над двумя код. комбинациями
    public static List<int> XorLists(List<int> first, List<int> second)
    {
        var firstInt = ListToBigInt(first);
        var secondInt = ListToBigInt(second);
        
        if (firstInt < secondInt)
            (first, second) = (second, first);

        var sizeMax = first.Count;
        
        for (int i = 0; i < second.Count; i++)
            if (first[i] == 1 && second[i] == 1)
                first[i] = 0;
            else
                first[i] = second[i] | first[i];

        while (first.Count < sizeMax)
            first.Add(0);
        
        return first;
    }

    //Вычисление модуля
    public static BigInteger CalcModule(BigInteger combin, BigInteger G)
    {
        var combinList = BigIntToList(combin);
        var gList = BigIntToList(G);
        while (combinList.Count >= gList.Count)
        {
            var bigInt = ListToBigIntCount(ref combinList, gList.Count);
            var list = DivLists(BigIntToList(bigInt), gList);
            combinList.AddRange(TrimList(list));
            TrimList(combinList);
        }
        return ListToBigInt(combinList);
    }

    //Генерация образующего многочлена
    public static BigInteger CalcCreatedG(int n, int r)
    {
        var G = new BigInteger(Math.Pow(10, r) + 1);
        var del = new BigInteger(Math.Pow(10, n) + 1);
        while (CountOne(G) < 3 && CalcModule(del, G) != 0)
        {
            G += 10;
            FixBinaryInput(ref G);
        }

        return G;
    }

    //Сдвиг влево
    public static List<int> ShiftLeft(List<int> val)
    {
        val.Insert(0,val[^1]);
        val.RemoveAt(val.Count - 1);
        return val;
    }

    //Сдвиг вправо
    public static List<int> ShiftRight(List<int> val)
    {
        val.Insert(val.Count,val[0]);
        val.RemoveAt(0);
        return val;
    }

    // Исправляет ошибки
    public static List<int>? FixMsg(BigInteger val, BigInteger G, int correctAbility)
    {
        int count = 0;
        var list = BigIntToList(val);
        var result = CalcModule(val,G); // Вычисляем модуль
        if (CountOne(result) == 0) // Если ошибок нет
            return BigIntToList(val);

        // Вычисление модуля деления и сдвиг до тех пор, пока кол-во 1 будет больше correctAbility
        // или количество сдвигов меньше количества символов в коде
        while (count < list.Count && CountOne(result) > correctAbility)
        {
            result = CalcModule(ListToBigInt(list),G);
            if (CountOne(result) <= 1) continue;
            list = ShiftLeft(list);
            count++;
        }
        
        if (count  == list.Count) // Если мы обошли всё число
            return null;

        // Складываем остаток с сдвинутой код. комбинацией
        list = XorLists(list, BigIntToList(result));
        for(; count > 0; count--)
            list = ShiftRight(list);
        return list;
    }

    // Генерация красивого вывода
    public static string СreatePolynomialView(BigInteger val) 
    {
        StringBuilder builder = new();
        var list = BigIntToList(val);
        for (var i = list.Count - 1; i >= 0; i--) 
            if (i == list.Count - 1) 
                builder.Append($"x^({i})");
            else if (i == 0 && list[i] == 1)
                builder.Append($" + {list[i]}");
            else if (list[i] == 1) 
                builder.Append($" + x^({i})");
        return builder.ToString();
    }

    // Разворачивает вход. комбинацию
    // BigInteger reverseBigInt(BigInteger val)
    // {
    //     var digits = BigIntToList(val);
    //     var newValue = new BigInteger(0);
    //     for (int i = 0; i < digits.Count; i++)
    //         newValue = newValue * 10 + digits[i];
    //     return newValue;
    // }

    public static string ConvertToString(List<int> list)
    {
        string str = "";
        for(int i = list.Count - 1; i >= 0; i--)
            str += list[i];
        return str;
    }
    public static string CreatePolynomialView(BigInteger val)
    {
        StringBuilder builder = new();
        var list = BigIntToList(val);
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (i == list.Count - 1)
            {
                builder.Append($"x^({i})");
            }
            else if (i == 0 && list[i] == 1)
            {
                builder.Append(" + ");
                builder.Append(list[i].ToString());
            }
            else if (list[i] == 1)
            {
                builder.Append(" + ");
                builder.Append($"x^({i})");
            }
        }
    
        return builder.ToString();
    }

    public static BigInteger ReverseBigInt(BigInteger val)
    {
        var digits = BigIntToList(val);
        var newValue = new BigInteger(0);
        for (int i = 0; i < digits.Count; i++)
            newValue = newValue * 10 + digits[i];
        return newValue;
    }

    public static bool IsBinary(BigInteger val)
    {
        var list = BigIntToList(val);
        foreach (var item in list)
        {
            if (item != 0 && item != 1)
                return false;
        }

        return true;
    }

    public static string ListToString(List<int> list)
    {
        StringBuilder builder = new StringBuilder();
        for (var i = list.Count - 1; i >= 0; i--) 
            builder.Append(list[i]);
        return builder.ToString();
    }
}