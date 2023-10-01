export interface ActionResultResponse<T> {
    actionSuccess: boolean;
    actionData: T;
    errors: string[];
  }
