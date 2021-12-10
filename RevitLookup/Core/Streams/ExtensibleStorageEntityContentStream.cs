using System.Collections;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitLookup.Core.RevitTypes;
using Double = RevitLookup.Core.RevitTypes.Double;
using ElementId = Autodesk.Revit.DB.ElementId;
using Enumerable = RevitLookup.Core.RevitTypes.Enumerable;
using Exception = System.Exception;
using Object = RevitLookup.Core.RevitTypes.Object;
using String = RevitLookup.Core.RevitTypes.String;

namespace RevitLookup.Core.Streams;

public class ExtensibleStorageEntityContentStream : IElementStream
{
    private readonly ArrayList _data;
    private readonly Document _document;
    private readonly Entity _entity;

    public ExtensibleStorageEntityContentStream(Document document, ArrayList data, object elem)
    {
        _document = document;
        _data = data;
        _entity = elem as Entity;
    }

    public void Stream(Type type)
    {
        if (type != typeof(Entity) || _entity is null || !_entity.IsValid()) return;
        if (!_entity.ReadAccessGranted())
            _data.Add(new RevitTypes.Exception("<Extensible storage Fields>", new Exception("Doesn't have access to read extensible storage data")));

        var fields = _entity.Schema.ListFields();
        if (fields.Count == 0) return;
        _data.Add(new ExtensibleStorageSeparator());
        foreach (var field in fields) StreamEntityFieldValue(field);
    }

    private void StreamEntityFieldValue(Field field)
    {
        try
        {
            var getEntityValueMethod = GetEntityFieldValueMethod(field);
            var valueType = GetFieldValueType(field);
            var genericGet = getEntityValueMethod.MakeGenericMethod(valueType);
            var fieldSpecType = field.GetSpecTypeId();
            var unit = UnitUtils.IsMeasurableSpec(fieldSpecType) ? UnitUtils.GetValidUnits(field.GetSpecTypeId())[0] : UnitTypeId.Custom;
            var parameters = getEntityValueMethod.GetParameters().Length == 1
                ? new object[] {field}
                : new object[] {field, unit};

            var value = genericGet.Invoke(_entity, parameters);
            AddFieldValue(field, value);
        }
        catch (Exception ex)
        {
            _data.Add(new RevitTypes.Exception(field.FieldName, ex));
        }
    }

    private Type GetFieldValueType(Field field)
    {
        switch (field.ContainerType)
        {
            case ContainerType.Simple:
                return field.ValueType;
            case ContainerType.Array:
                var generic = typeof(IList<>);
                return generic.MakeGenericType(field.ValueType);
            case ContainerType.Map:
                var genericMap = typeof(IDictionary<,>);
                return genericMap.MakeGenericType(field.KeyType, field.ValueType);
            default:
                throw new NotSupportedException();
        }
    }

    private void AddFieldValue(Field field, object value)
    {
        try
        {
            if (field.ContainerType != ContainerType.Simple)
            {
                _data.Add(new Enumerable(field.FieldName, value as IEnumerable));
            }
            else if (field.ValueType == typeof(double))
            {
                _data.Add(new Double(field.FieldName, (double) value));
            }
            else if (field.ValueType == typeof(string))
            {
                _data.Add(new String(field.FieldName, value as string));
            }
            else if (field.ValueType == typeof(XYZ))
            {
                _data.Add(new Xyz(field.FieldName, value as XYZ));
            }
            else if (field.ValueType == typeof(UV))
            {
                _data.Add(new Uv(field.FieldName, value as UV));
            }
            else if (field.ValueType == typeof(int))
            {
                _data.Add(new Int(field.FieldName, (int) value));
            }
            else if (field.ValueType == typeof(ElementId))
            {
                _data.Add(new RevitTypes.ElementId(field.FieldName, value as ElementId, _document));
            }
            else if (field.ValueType == typeof(Guid))
            {
                var guidValue = (Guid) value;

                _data.Add(new String(field.FieldName, guidValue.ToString()));
            }
            else
            {
                _data.Add(new Object(field.FieldName, value));
            }
        }
        catch (Exception ex)
        {
            _data.Add(new RevitTypes.Exception(field.FieldName, ex));
        }
    }

    private static MethodInfo GetEntityFieldValueMethod(Field field)
    {
        return typeof(Entity)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name == nameof(Entity.Get) && x.IsGenericMethod)
            .Single(x => IsGetByFieldMethod(x, field));
    }

    private static bool IsGetByFieldMethod(MethodInfo methodInfo, Field field)
    {
        var parameters = methodInfo.GetParameters();
        var fieldSpecType = field.GetSpecTypeId();

        if (UnitUtils.IsMeasurableSpec(fieldSpecType) || fieldSpecType == SpecTypeId.Custom)
        {
            var firstParameter = parameters[0];
            var lastParameter = parameters[parameters.Length - 1];
            return parameters.Length == 2 && firstParameter.ParameterType == typeof(Field) && lastParameter.ParameterType == typeof(ForgeTypeId);
        }

        return parameters.Length == 1 && parameters.Single().ParameterType == typeof(Field);
    }
}