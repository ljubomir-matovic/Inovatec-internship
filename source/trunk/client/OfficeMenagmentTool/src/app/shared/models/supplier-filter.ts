export interface SupplierFilter {
    Name?: string | null,
    PhoneNumber?: string | null,
    Country?: string | null,
    City?: string | null,
    Address?: string | null,
    PageNumber: number,
    PageSize: number,
    SortField?: string | null,
    SortOrder: -1 | 1,
}