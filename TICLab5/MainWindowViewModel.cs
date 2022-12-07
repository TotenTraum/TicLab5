using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace TICLab5;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Кодовая комбинация
    /// </summary>
    public string InfoCombin
    {
        get => _infoCombin;
        set => _infoCombin = value;
    }

    /// <summary>
    /// Кодовая комбинация в виде полинома
    /// </summary>
    public string PolynomView
    {
        get => _polynomView;
        private set => SetField(ref _polynomView, value);
    }

    /// <summary>
    /// Количество корректирующих разрядов
    /// </summary>
    public string R
    {
        get => _r != 0 ? _r.ToString() : "";
        private set => SetField(ref _r, int.Parse(value));
    }

    /// <summary>
    /// Образующий многочлен
    /// </summary>
    public string GPolynomCombin
    {
        get => _gPolynomCombin;
        private set => SetField(ref _gPolynomCombin, value);
    }
    
    /// <summary>
    /// Образующий многочлен в виде полинома
    /// </summary>
    public string GPolynomView
    {
        get => _gPolynomView;
        private set => SetField(ref _gPolynomView,
            CrcUtils.CreatePolynomialView(BigInteger.Parse(value)));
    }

    /// <summary>
    /// Результат кодирования
    /// </summary>
    public string ResultPolynomCombin
    {
        get => _resultPolynomCombin;
        set => SetField(ref _resultPolynomCombin, value);
    }
    
    /// <summary>
    /// Результат кодирования в виде полинома
    /// </summary>
    public string ResultPolynomView
    {
        get => _resultPolynomView;
        private set => SetField(ref _resultPolynomView, CrcUtils.CreatePolynomialView(BigInteger.Parse(value)));
    }
    
    /// <summary>
    /// Сломанная кодовая комбинация
    /// </summary>
    public string FakeCode
    {
        get => _fakeCode;
        set => SetField(ref _fakeCode, value);
    }
    
    /// <summary>
    /// Исправленная кодовая комбинация
    /// </summary>
    public string FixedCode
    {
        get => _fixedCode;
        private set => SetField(ref _fixedCode, value);
    }

    #region command
    /// <summary>
    /// Этап кодирования
    /// </summary>
    public ActionCommand Start1Command => new((_) =>
    {
        // Парсим кодовую комбинацию 
        var num = CrcUtils.parseString(InfoCombin);
        PolynomView = CrcUtils.СreatePolynomialView(CrcUtils.ListToBigInt(num));
        // Вычисление корректирующих разрядов
        R = CrcUtils.CalculateCodeDistance(num.Count).ToString();
        var n = _r + InfoCombin.Length;
        // Вычисление образующего многочлена
        GPolynomCombin = CrcUtils.CalcCreatedG(n, _r).ToString();
        // Приведение к виду полинома
        GPolynomView = GPolynomCombin;
        // Создаем комбинацию num * 10^(r-1) 
        var cmb = new List<int>(num);
        for (int i = 0; i < _gPolynomCombin.Length - 1; i++)
            cmb.Insert(0, 0);
        // Вычисляем остаток деления комбинации num * 10^(r-1) на образующий многочлен 
        var module = CrcUtils.CalcModule(CrcUtils.ListToBigInt(cmb), BigInteger.Parse(_gPolynomCombin));
        // Суммируем остаток с комбинацией num * 10^(r-1)
        ResultPolynomCombin = CrcUtils.ListToString(CrcUtils.AddLists(cmb, CrcUtils.BigIntToList(module)));
        // Приведение к виду полинома
        ResultPolynomView = ResultPolynomCombin;
        FakeCode = ResultPolynomCombin;
    });
    
    /// <summary>
    /// Этап декодирования
    /// </summary>
    public ActionCommand Start2Command => new((_) =>
    {
        var tmp = CrcUtils.FixMsg(CrcUtils.parseString(_fakeCode), BigInteger.Parse(_gPolynomCombin), 1);
        if(tmp != null)
            FixedCode = CrcUtils.ListToString(tmp);
    });

    #endregion
    
    #region private

    private string _infoCombin = "";
    private int _r;
    private string _gPolynomCombin = "";
    private string _gPolynomView = "";
    private string _resultPolynomCombin = "";
    private string _resultPolynomView = "";
    private string _polynomView = "";
    private string _fakeCode = "";
    private string _fixedCode = "";
    
    #endregion
    
    #region MVVM_part
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
    #endregion
}