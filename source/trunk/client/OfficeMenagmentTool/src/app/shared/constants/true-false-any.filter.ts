export enum TrueFalseAnyFilter {
    True,
    False,
    Any
}

export const TRUE_FALSE_ANY_FILTER_OPTIONS = [
    { id: TrueFalseAnyFilter.Any, name: "Any", value: null, numericalValue: 2 },
    { id: TrueFalseAnyFilter.True, name: "TrueLabel", value: true, numericalValue: 1 },
    { id: TrueFalseAnyFilter.False, name: "FalseLabel", value: false, numericalValue: 0 }
];