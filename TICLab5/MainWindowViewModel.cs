using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace TICLab5;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public class ActionCommand : ICommand
    {
        private Action<object>? _action;

        public ActionCommand(Action<object> exec)
        {
            _action = exec;
        }

        public bool CanExecute(object? parameter)
        {
            return _action != null;
        }

        public void Execute(object? parameter)
        {
            if(_action != null)
                _action(parameter?? "Null");
        }

        public event EventHandler? CanExecuteChanged;
    }
    public string InfoCombin { get; set; }

    private string _polynomView = "";
    public string PolynomView
    {
        get { return _polynomView; }
        set { _polynomView = value; OnPropertyChanged(); }
    }
    public int r { get; set; }

    public string R
    {
        get
        {
            if (r != 0)
                return r.ToString();
            return "";
        }
        set
        {
            r = int.Parse(value);
            OnPropertyChanged();
        }
    }

    public string GPolynom
    {
        get
        {
            if (!string.IsNullOrEmpty(_gPolynom))
                return CrcUtils.CreatePolynomialView(BigInteger.Parse(_gPolynom)) + " | " + _gPolynom;
            else
                return _gPolynom;
        }
        set { _gPolynom = value; OnPropertyChanged(); }
    }
    private string _gPolynom { get; set; }
    private string _testPolynom { get; set; }

    private bool _invert = false;
    public bool Invert
    {
        get { return _invert; }
        set { _invert = value; OnPropertyChanged(); }
    }


    public string TestPolynom
    {
        get { return _testPolynom;}
        set { _testPolynom = value; OnPropertyChanged(); }
    }

    public string TestPolynomCombin
    {
        get { return _testPolynomCombin;}
        set
        {
            _testPolynomCombin = value;
            OnPropertyChanged();
        }
    }
    private string _testPolynomCombin { get; set; }
    public ActionCommand start1Command { get; set; }
    public ActionCommand start2Command { get; set; }

    private BigInteger _fakeCode = 0;

    public string FakeCode
    {
        get
        {
            return _fakeCode.ToString();
        }
        set
        {
            _fakeCode = BigInteger.Parse(value);
            OnPropertyChanged();
        }
    }
    
    private string _fixedCode = "";

    public string FixedCode
    {
        get => _fixedCode;
        set
        {
            _fixedCode = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel()
    {
        InfoCombin = "";
        PolynomView = "";
        r = 0;
        GPolynom = "";
        
        start1Command = new ActionCommand(x => Start1_OnClick());
        start2Command = new ActionCommand(x => Start2_OnClick());
    }

    public void Start1_OnClick()
    {
        var num = BigInteger.Parse(InfoCombin);
        if(! CrcUtils.IsBinary(num))
            return;
        
        PolynomView =  CrcUtils.СreatePolynomialView(num);
        R = CrcUtils.CalculateCodeDistance(CrcUtils.CalculateDigitsCount(num)).ToString();
        GPolynom = CrcUtils.CalcCreatedG(r+ InfoCombin.Length,r).ToString();
        var Combination = num * (BigInteger)Math.Pow(10, CrcUtils.CalculateDigitsCount(BigInteger.Parse(_gPolynom)) - 1);
        var list = CrcUtils.BigIntToList(num);
        var module = CrcUtils.CalcModule(Combination, BigInteger.Parse(_gPolynom));
        TestPolynomCombin = CrcUtils.ListToBigInt(CrcUtils.AddLists(CrcUtils.BigIntToList(Combination), CrcUtils.BigIntToList(module))).ToString();
        TestPolynom = CrcUtils.CreatePolynomialView(CrcUtils.ListToBigInt(CrcUtils.AddLists(CrcUtils.BigIntToList(Combination), CrcUtils.BigIntToList(module))));
        FakeCode = TestPolynomCombin;
    }

    public void Start2_OnClick()
    {
        var tmp = CrcUtils.FixMsg(_fakeCode, BigInteger.Parse(_gPolynom), 1);
        if(tmp != null)
            FixedCode = CrcUtils.ListToString(tmp);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}