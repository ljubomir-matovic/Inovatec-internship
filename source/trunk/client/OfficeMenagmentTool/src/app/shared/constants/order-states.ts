export enum OrderState {
    Pending,
    InProgress,
    Done,
    Canceled
}

export const ORDER_STATES = [
    { id: OrderState.Pending, name: "Pending", default: true },
    { id: OrderState.InProgress, name: "In Progress", default: true },
    { id: OrderState.Done, name: "Done", default: false },
    { id: OrderState.Canceled, name: "Canceled", default: false }
];