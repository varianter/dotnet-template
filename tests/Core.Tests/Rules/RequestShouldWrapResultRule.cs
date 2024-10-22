using FluentResults;
using MediatR;
using Mono.Cecil;
using NetArchTest.Rules;

namespace Core.Tests.Rules;

public class RequestShouldWrapResultRule : ICustomRule
{
    private static readonly Type ResultType = typeof(Result);
    private static readonly Type ResultGenericType = typeof(Result<>);
    private static readonly Type RequestGenericType = typeof(IRequest<>);
    
    public bool MeetsRule(TypeDefinition typeDefinition)
    {
        // Should be a single interface that is IRequest<T>
        var requestType = typeDefinition.Interfaces
            .Single(i => 
                TypeReferenceMatchesType(i.InterfaceType, RequestGenericType))
            .InterfaceType as GenericInstanceType;
        
        var responseType = requestType.GenericArguments[0];

        // Response type should be Result or Result<T>
        return TypeReferenceMatchesType(responseType, ResultType)
               || TypeReferenceMatchesType(responseType, ResultGenericType);
    }
    
    private static bool TypeReferenceMatchesType(TypeReference typeReference, Type type)
    {
        return typeReference.Name == type.Name && typeReference.Namespace == type.Namespace;
    }
}