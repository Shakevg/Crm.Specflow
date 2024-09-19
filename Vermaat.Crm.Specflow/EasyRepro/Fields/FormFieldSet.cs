using System.Collections.Generic;
using System.Linq;

namespace Vermaat.Crm.Specflow.EasyRepro.Fields
{
    internal class FormFieldSet
    {
        private Dictionary<string, Dictionary<string, List<FormField>>> _formFields;

        public FormFieldSet()
        {
            _formFields = new Dictionary<string, Dictionary<string, List<FormField>>>();
        }

        public void Add(FormField field, FormCellTabAndSectionContext formCellTabAndSectionContext)
        {
            var tabNotNull = formCellTabAndSectionContext.TabName ?? string.Empty;
            var sectionNotNull = formCellTabAndSectionContext.SectionName ?? string.Empty;

            if (!_formFields.TryGetValue(tabNotNull, out var sectionDic))
            {
                sectionDic = new Dictionary<string, List<FormField>>();
                _formFields.Add(tabNotNull, sectionDic);
            }

            if (!sectionDic.TryGetValue(sectionNotNull, out var fieldList))
            {
                fieldList = new List<FormField>();
                sectionDic.Add(sectionNotNull, fieldList);
            }

            tabNotNull = formCellTabAndSectionContext.TabLabel ?? string.Empty;
            sectionNotNull = formCellTabAndSectionContext.SectionLabel ?? string.Empty;

            if (!_formFields.TryGetValue(tabNotNull, out var sectionDicNew))
            {
                sectionDicNew = new Dictionary<string, List<FormField>>();
                _formFields.Add(tabNotNull, sectionDicNew);
            }

            if (!sectionDicNew.TryGetValue(sectionNotNull, out var fieldListNew))
            {
                fieldListNew = new List<FormField>();
                sectionDicNew.Add(sectionNotNull, fieldListNew);
            }

            fieldList.Add(field);
            fieldListNew.Add(field);
        }

        public FormField Get()
        {
            return _formFields.Values.First().Values.First().First();
        }

        public FormField Get(string tab, bool allowNull)
        {
            if (_formFields.TryGetValue(tab ?? string.Empty, out var sectionDic))
            {
                return sectionDic.Values.First().First();
            }

            if (allowNull)
                return null;
            else
                throw new TestExecutionException(Constants.ErrorCodes.FIELD_NOT_IN_TAB, tab);
        }

        public FormField Get(string tab, string section, bool allowNull)
        {
            if (_formFields.TryGetValue(tab ?? string.Empty, out var sectionDic))
            {
                if (sectionDic.TryGetValue(section ?? string.Empty, out var fieldList))
                {
                    return fieldList.First();
                }
                else if (allowNull)
                    return null;
                else
                    throw new TestExecutionException(Constants.ErrorCodes.FIELD_NOT_IN_SECTION, section);
            }
            else if (allowNull)
                return null;
            else
                throw new TestExecutionException(Constants.ErrorCodes.FIELD_NOT_IN_TAB, tab);

        }
    }
}
