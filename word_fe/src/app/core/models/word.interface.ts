import { IRegion } from "./region.interface";

export interface IAddWord {
    email: string;
    region: { name: string };
    value: string;
}

export interface IAddWordResponse {
    myRegionStats: IWordStatistic;
    allRegionStats: IWordStatistic;
    region: IRegion;
}

export interface IWordStatistic {
    mostPopularWord: IWordGroup;
    yourWord: IWordGroup;
    closestWords: IWordGroup[];
}

export interface IWordGroup {
    value: string;
    count: number;
}