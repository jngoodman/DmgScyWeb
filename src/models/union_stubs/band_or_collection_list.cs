using OneOf;

namespace DmgScy;

[GenerateOneOf]
public partial class BandOrCollectionList: OneOfBase<List<Band>, List<Collection>>{};

// These stubs effectively are here to save the order of types in the union type and should not be changed.
