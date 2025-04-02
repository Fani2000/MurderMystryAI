export interface UserInput {
    name: string;
}

export interface StoryResult {
    fullStory: string;
    killer: string;
    userStories: { [name: string]: string };
}

export interface GenerateMurderMysteryResponse {
    generateMurderMystery: StoryResult;
}