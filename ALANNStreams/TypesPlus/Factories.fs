﻿ (*
 * The MIT License
 *
 * Copyright 2018 The ALANN2018 authors.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *)

module Factories

open System
open Types
open Evidence
open TermUtils
open SystemState

let makeLTI depth = if depth = SearchDepth.Deep then Params.DEEP_LTI else Params.SHALLOW_LTI

// Event factories
let makeEventFromBelief eb =
    {EventType = Belief
     Term = eb.Belief.Term
     TV = Some eb.Belief.TV
     AV = {STI = eb.Attention; LTI = makeLTI eb.Depth}
     Stamp = eb.Belief.Stamp
     Solution = None}

// Belief factories

let makeBeliefFromEvent (e : Event) =
    match e with
    | {EventType = Belief; TV = Some tv} -> {Term = e.Term; TV = tv; Stamp = e.Stamp}
    | _ -> failwith "makeBeliefFromEvent: Event is not Belief"

let makeGoalFromEvent (e : Event) =
    match e with
    | {EventType = Goal; TV = Some tv} -> {Term = e.Term; TV = tv; Stamp = e.Stamp}
    | _ -> failwith "makeBeliefFromEvent: Event is not Belief"

let makeVirtualBelief term =
    let now = SystemTime()
    let stamp = {OccurenceTime = now
                 SC = 1
                 Evidence = []
                 Source = Virtual}

    {Term = term; TV = {F = 0.0f; C = 0.5f}; Stamp = stamp}    

// EventBelief factories

let makeInferredEvent eb (term, tv) =
    let temporalDistanceDiscount eb = 
        let t1 = eb.Event.Stamp.OccurenceTime
        let t2 = eb.Belief.Stamp.OccurenceTime
        let d = abs(t1 - t2)
        float32(Math.Pow(0.999, float(d)))

    let stamp1 = eb.Event.Stamp
    let stamp2 = eb.Belief.Stamp
    let now = SystemTime()
    let stamp = {OccurenceTime = now
                 SC = syntacticComplexity term 
                 Evidence = merge stamp1.Evidence stamp2.Evidence
                 Source = Derived}

    {EventType = Belief; Term = term; TV = Some tv; AV = {STI = eb.Attention; LTI = makeLTI eb.Depth}; Stamp = stamp; Solution = None}

let makeInferredFromQuestionEvent eb (term, tv) =
    let now = SystemTime()
    let stamp = {OccurenceTime = now
                 SC = syntacticComplexity term 
                 Evidence = eb.Belief.Stamp.Evidence
                 Source = Derived}

    {EventType = Belief; Term = term; TV = Some tv; AV = {STI = eb.Attention; LTI = makeLTI eb.Depth}; Stamp = stamp; Solution = None}

let makeStructuralEvent eb (term, tv) =
    let now = SystemTime()
    let stamp = {OccurenceTime = now
                 SC = syntacticComplexity term 
                 Evidence = eb.Event.Stamp.Evidence
                 Source = Derived}

    {EventType = Belief; Term = term; TV = Some tv; AV = {STI = eb.Attention; LTI = makeLTI eb.Depth}; Stamp = stamp; Solution = None}

let makeQuestionEvent (eb : EventBelief) term =
    let now = SystemTime()
    let stamp = {OccurenceTime = now
                 SC = syntacticComplexity term                 
                 Evidence = eb.Belief.Stamp.Evidence
                 Source = Derived}

    {EventType = Question; Term = term; TV = None; AV = {STI = eb.Attention; LTI = makeLTI eb.Depth}; Stamp = stamp; Solution = None}

let makeQuestionStructuralEvent (eb : EventBelief) term =
    let now = SystemTime()
    let stamp = {OccurenceTime = now
                 SC = syntacticComplexity term 
                 Evidence = eb.Belief.Stamp.Evidence
                 Source = Derived}

    {EventType = Question; Term = term; TV = None; AV = {STI = eb.Attention; LTI = makeLTI eb.Depth}; Stamp = stamp; Solution = None}


let makeQuestEvent (eb : EventBelief) term =
    let now = SystemTime()
    let stamp = {OccurenceTime = now
                 SC = syntacticComplexity term 
                 Evidence = []
                 Source = Derived}

    {EventType = Quest; Term = term; TV = None; AV = {STI = eb.Attention; LTI = makeLTI eb.Depth}; Stamp = stamp; Solution = None}

let makeGoalEvent (eb : EventBelief) (term, tv) =
    let now = SystemTime()
    let stamp = {OccurenceTime = now
                 SC = syntacticComplexity term 
                 Evidence = merge eb.Event.Stamp.Evidence eb.Belief.Stamp.Evidence
                 Source = Derived}

    {EventType = Goal; Term = term; TV = Some tv; AV = {STI = eb.Attention; LTI = makeLTI eb.Depth}; Stamp = stamp; Solution = None}

