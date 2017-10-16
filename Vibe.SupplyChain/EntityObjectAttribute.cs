using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain
{
    public enum Mandate { Required, Optional}
    public enum Accessibility { Auto, NoView, Edit, EditLarge}
    public enum FooterMode { None, Sum }
    public enum BGColor { None, Green, Red, Yellow }
    public enum ConstraintImage { None, Success, Fail }
    public enum CalculatedValue { None, Image, Text }
    public class EntityObjectAttribute : Attribute
    {
        int _seq = 999;
        public int Sequence { get { return _seq; } set { _seq = value; } }
        public string DisplayLabel { get; set; }
        public string DisplayPrefix { get; set; }
        public int MaxLength { get; set; }
        public bool IsConstraint { get; set; }
        public Accessibility Accessibility { get; set; }
        public Mandate Mandate { get; set; }
        public CalculatedValue CalculatedValue { get; set; }
        public BGColor SuccessColor { get; set; }
        public BGColor FailColor { get; set; }
        public ConstraintImage SuccessImage { get; set; }
        public ConstraintImage FailImage { get; set; }
        public string SuccessMessage { get; set; }
        public string FailMessage { get; set; }
        public FooterMode FooterMode { get; set; }
        public string SelectOptions { get; set; }
        public string SelectText { get; set; }
        public string SelectEntityFilterValue { get; set; }
        public string SelectElementFilterValue { get; set; }
        public Operand SelectOperand { get; set; }
        public string PassCriteria { get; set; }
        public int Width { get; set; }
        public string ShowChart { get; set; }
        public EntityObjectAttribute()
        {
            Accessibility = Accessibility.Auto;
            Mandate = Mandate.Optional;
            FooterMode = FooterMode.None;
            SuccessColor = BGColor.None;
            FailColor = BGColor.None;
            SuccessImage = ConstraintImage.None;
            FailImage = ConstraintImage.None;
            CalculatedValue = CalculatedValue.None;
        }
    }
}
