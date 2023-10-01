export enum CommentTypes {
    Regular,
    Edited,
    StateChange,
    Tag
}

export const COMMENT_TYPES = [
    { id: CommentTypes.Regular, name: "Regular" },
    { id: CommentTypes.Edited, name: "Edited" },
    { id: CommentTypes.StateChange, name: "StateChange" },
    { id: CommentTypes.Tag, name: "Tag" }
];