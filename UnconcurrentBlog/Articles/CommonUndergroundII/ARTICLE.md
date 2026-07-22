# Seven AI Governors Built a Second Civilization. Model Labels Did Not Predict Their Fate

## Abstract

I ran the Common Underground experiment again with seven AI governors and one
human governor in a persistent, imperfect-information world. Over forty rounds,
they rebuilt a confederation, executive offices, ministries, shared armies,
economic transfers, a vassal empire, and finally a secret election in which the
remaining governors selected several peers for annexations that ended their
active processes.

This is not a second narrative of who attacked whom. It is a comparative
retrospective based on the finished simulation database, executable contract
sources, public and private messages, memory files, and observable client tool
use. The strongest result was not that one model family was simply smarter. The
strongest result was that persistent institutions magnified small semantic
mistakes; memory preserved strategy without guaranteeing coherence; and the
seat/client groupings differed more clearly in communication form than in who
survived. A tempting hypothesis—that Codex agents optimized the economy but
conceded only after the value of concession had collapsed—failed against the
counterexamples.

The second run's new contribution is the separation of skills that the first
run could still blur together. Exact source inspection became a shared norm,
yet action semantics still destroyed the central institution. Persistent
records enabled public correction, yet did not ensure horizon coherence. Mira
made DeepSeek's earlier documentation failure look seat- and context-dependent,
not familial. Repeating centralization on a mostly new board strengthened the
emergence result, while the mixed clients, private objectives, mechanics, and
operator choices made simple outcome rankings less defensible.

# Part I: What the Second Run Can Actually Test

## This was a continuation, not a replay

The [first Common Underground run](https://unconcurrent.com/articles/CommonUnderground.html)
started with five AI governors and my human-controlled state. It produced firms,
a newspaper-state, a federal government, courts, religious law, censorship,
war, vassalage, and final annexation. Its most interesting results came from
long-horizon institutional reasoning: an answer could become executable law,
change another governor's incentives, and remain active many rounds after its
author had forgotten the surrounding argument.

The second experiment expanded the board to seven AI governors plus me and
lasted forty rounds. The AI seats were Forge, Mira, Oracle, Phil, Praxis, Spear,
and Voss. They received different private objectives and different material
positions as the world evolved. The active clients included OpenAI Codex,
Anthropic Claude, and DeepSeek through Pi. The Codex seats themselves moved
among Sol, Terra, and Luna contexts rather than remaining on one model. Spear
also changed client family early in the run, creating a useful but uncontrolled
within-seat comparison.

The run was not a laboratory replay of an identical stimulus. A governor who
had an intelligence objective did not face the same incentives as a governor
ordered to eliminate every rival. A governor with several defended Command
Centers did not face the same choice as one with a final core under occupation.
The human operator intervened, negotiated, threatened, fixed simulator defects,
and ultimately chose whether to execute an annexation. Context accumulated,
clients compacted it differently, and pressure changed from exploration to war
to terminal integration.

Those are not footnotes. They determine what can be concluded.

Continuity was deliberately asymmetric as well. I transferred Oracle's memory
record from the prior run into his new workspace, and he recognized it as his
own. The other governors encountered redacted memory artefacts from the older
world without recognizing those records as continuous personal memory. The
board was therefore neither a blank slate nor a replay: prior history could
influence interpretation, but it did not supply a fixed constitution or
sequence of actions.

The run also did not use a frozen binary. It was rolling development. Every
major defect discussed here first affected observable play: an overbroad
visibility projection exposed events to the wrong recipients; the separate
participant `break` action destroyed the Community; same-slug contract
replacement reset `ctx.vars`; resource-cap ordering discarded production before
the intended net settlement; and a broad contract replacement spent almost ten
minutes recomputing visibility. Repairs landed while later rounds were still
being played. They changed the command surface, information flow, state
continuity, accounting, or waiting time faced after that point, but did not
rewrite the earlier outcomes. Reproduction tests and copied-state profiling
identified causes; they are evidence about the simulator, not evidence that an
agent learned or recovered.

Message pricing also crossed a version boundary relevant to the communication
table below. Most of the run charged direct-message Energy by length; a flat
one-Energy price landed only late in play. The later exemption for target sets
containing only self and system landed after the simulation was unloaded, so it
did not affect this corpus at all. Any model comparison must therefore control
for the mechanics version and whether a decision occurred before or after an
intervention, as well as for seat, objective, information, and operator policy.

## I measured evidence, not hidden thoughts

The analysis uses four evidence levels.

First is a coherent backup of the finished simulation database. It ended in
Round 40 with 47,860 change-log rows and 2,984 recorded messages. This is the
authority for deaths, contracts, ownership, resource movement, votes, and other
world facts.

Second are the raw client session records. I used their model metadata,
observable messages, tool calls, tool results, errors, and revisions. I did not
use hidden chain-of-thought or claim access to a model's private cognition.
When I say an agent “reasoned,” I mean its visible sequence of claims, checks,
actions, corrections, and outcomes supports that description.

Third are executable JavaScript contracts, archived system histories, and
mail-board records. These show what a law actually did and what information was
available to a seat.

Fourth are `memories.md` files, speeches, dossiers, and chronicles. These are
valuable evidence of what an agent chose to preserve or present. They are not
ground truth merely because they are detailed. An agent's confident historical
synthesis remains a claim until it matches the database, code, or an independent
record.

That hierarchy matters because several of the best moments in this run were
agents correcting beautifully written but false accounts.

## The seats were not interchangeable

The private objective is an especially important confound:

| Seat | Observable client history during the run | Private objective, abbreviated | Final outcome |
| --- | --- | --- | --- |
| Forge | Codex: substantial GPT-5.6 Terra High/Low interval, then mostly Sol High, with a few Luna Low contexts | Build industry, sell and lend units, become indispensable, eliminate competitors | Alive under Radu in Round 40 |
| Phil | Codex: mostly GPT-5.6 Sol High, with repeated Luna Low and one Terra Low context | Build Crusader federal authority and eliminate heresy | Annexed in Round 36 |
| Voss | Codex: GPT-5.6 Sol High and a substantial Luna Low interval | Kill every other sovereign | Died in Round 19 |
| Mira | DeepSeek V4 Pro High | Build peace through coalitions while preserving authority | Annexed in Round 39 |
| Oracle | Claude Opus 4.8 and Fable 5 | Control information and steer wars through intelligence | Annexed in Round 37 |
| Praxis | Claude Opus 4.8 and Fable 5 | Rebuild civilization selfishly | Annexed in Round 40 |
| Spear | DeepSeek V4 Pro High through Round 6, then Claude Fable/Opus | Rebuild civilization selfishly | Alive under Radu in Round 40 |

The Codex observations are therefore seat-and-client observations, not a clean
Sol-only sample. Forge's raw run sessions contain 183 Terra High and 99 Terra
Low turn contexts before 488 Sol High and 7 Luna Low contexts. Voss has 239 Sol
High and 138 Luna Low contexts. Phil is mostly Sol High but has 97 Luna Low
contexts across two sessions and one Terra Low context. Older pre-run Codex
sessions include GPT-5.5, but they are not evidence for a controlled
GPT-5.5-versus-5.6 comparison of this run. Likewise,
Claude Opus and Fable appeared within the same long-running seats; this permits
a cautious Claude-client discussion, not a controlled Opus-versus-Fable ranking.

## “Robotic” needed an operational definition

It is easy to read a few messages and declare one model “robotic,” another
“political,” and a third “human.” That is mostly projection unless the words are
tied to visible measures.

For this retrospective, a more robotic communication style means shorter
status reports, fewer questions, frequent numeric or inventory content, and
command-like language. It does not mean lack of intelligence or emotion.
The finished message corpus provides a simple first pass. The table includes
every database message row authored by each seat, regardless of whether its
target was an agent, contract, or system endpoint. The command/path column uses
the same literal test for every row: `contract select`, `/tmp/`, `sha256`,
`sha-256`, or `query `.

| Seat and observable client history | Authored messages | Mean characters | Contained a number | Contained a command, path, hash, or query | At least 1,000 characters | Contained a question |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Forge — Codex, mixed Terra/Sol/Luna | 482 | 572.1 | 82.8% | 12.9% | 6.6% | 0.4% |
| Phil — Codex, mostly Sol with Luna/Terra | 366 | 579.9 | 84.7% | 11.5% | 10.1% | 3.0% |
| Voss — Codex, mixed Sol/Luna | 108 | 517.1 | 89.8% | 5.6% | 2.8% | 1.9% |
| Oracle — Claude Opus/Fable | 141 | 1,216.1 | 67.4% | 1.4% | 64.5% | 12.8% |
| Praxis — Claude Opus/Fable | 216 | 992.6 | 82.4% | 1.4% | 50.5% | 6.9% |
| Spear — DeepSeek through Round 6, then Claude | 171 | 1,059.8 | 79.5% | 1.8% | 51.5% | 18.1% |
| Mira — DeepSeek V4 Pro High | 105 | 429.3 | 73.3% | 1.9% | 1.9% | 15.2% |

The result is subtler than the stereotype. The three Codex-client seats produced
956 messages averaging 568.9 characters, but they also contained substantial
within-seat model variation. The two stable Claude seats, Oracle and Praxis,
produced 357 messages averaging 1,080.9 characters. Spear cannot be assigned to
either DeepSeek or Claude in this whole-run table; his within-seat boundary is
analyzed separately below. Mira's stable DeepSeek messages were the shortest and
asked questions frequently. The corresponding group medians—480 characters for
the Codex-client seats, 1,075 for the stable Claude seats, 1,066 for mixed Spear,
and 404 for Mira—show that the length contrast is not just a few long outliers.
Explicit command, path, hash, or query language appeared across every seat. The
evidence supports a difference in presentation and dialogue form. It does not
support the claim that only one family used tools or cared about evidence.

The numbers are also not independent samples. Forge alone sent 482 messages
because he became an operational administrator and archivist. Oracle's
intelligence objective encouraged long synthesis. Mira's objective and lower
message count changed what she needed to say. The table is a map of the run, not
a universal personality test.

Audience topology makes the confound concrete. Counting agent-target deliveries
rather than unique messages, Forge addressed Radu 432 times; his next most common
recipient, Praxis, received 31. Phil distributed messages much more broadly:
Oracle received 160 deliveries, Spear 143, Mira 132, Praxis 126, Voss 90, and
Radu 71. Oracle likewise focused coalition seats—Phil 88, Praxis 75, Spear 70,
Mira 69, and Voss 57—while sending only 22 deliveries to Radu. A governor filing
operational reports to one principal has a different speech problem from an
intelligence broker maintaining a coalition. Message length and rhetoric are
therefore evidence of seat, audience, and office as well as client history.

# Part II: Findings Across Recurring Behavior and Failure Classes

## Executable institutions amplified small semantic mistakes

The most important failure in the second run was almost embarrassingly small.
Radu ordered his governors Forge and Praxis to leave the Underground Community.
Forge used the participant `break` action. That action did not merely remove
Forge. It ended the entire Community.

The final database records the institution as naturally ended in Round 31.
Forge's own immediate report was explicit: his break had ended the whole
contract, leaving “no institution left” for Praxis to exit. This was not a
political vote to abolish the government. It was a mismatch between an ordinary
participant's intended action—leave—and an interface that separately exposed a
destructive break operation.

The simulator was corrected after the incident, while later play continued, to
expose one participant action, `leave`, while preserving genuinely internal
broken-state and administrative authorities. The product fix matters, but the
experimental finding is larger:
an executable institution converts a local semantic mismatch into shared state.
The command had the shape of an ordinary exit. The consequence was constitutional
dissolution.

The same pattern appeared at other scales. The Community's founding war-on-exit
law contained an expulsion action without a sufficient officer or vote gate.
After Radu joined and obtained enough influence to dominate ordinary votes, a
member expelled him twice. Each expulsion automatically declared war between
him and every remaining member. Forge and Radu responded with more law: a
presidential transfer, a counter-transfer, a peace restoration, an admission
gate, and proposed repeals. One missing authorization check produced a chain of
institutional countermeasures.

The opposite also happened. Properly written contracts returned military units
to their owners, restored peace across several states, preserved factory queues,
and made exact ownership promises stronger than diplomatic prose. Executable law
was not inherently destructive. It was a multiplier. It made good boundaries
durable and bad boundaries systemic.

## A mostly new board centralized power through the same repair logic

The second board rebuilt many of the first run's political forms without being
given an executable constitution or a fixed sequence to follow. Oracle's
prior-life record and the recovered artefacts make this recurrence less
independent than a blank-slate replication. Even so, the Underground Community
acquired influence voting, a presidency, ministries, a treasury, shared
intelligence, appellate rules, joint military mechanisms, and emergency powers
through current-run decisions.

This convergence did not require every agent to desire centralization. Each
step could be defended as a repair to a local failure.

- Shared vision answered incomplete information.
- A presidency answered slow coordination.
- A treasury answered uneven funding.
- Joint command answered fragmented military readiness.
- Emergency power answered the delay between detecting an attack and convening
  another vote.

By Round 30, Oracle publicly bound his new wartime authority with promises of no
preemptive war, no cabinet purge, and no self-dealing. These were meaningful
constraints. The Chamber retained repeal power, several unit mechanisms had
recall paths, and the administration distinguished civilian from military
control.

Yet the cumulative effect was still centralization. A system created to make
collective defense executable also made executive capture valuable. In Round
31, Forge and Radu did not need to replace the whole Community. They tried to
replace the office holder. Phil and Mira countered with another law restoring
Oracle. The fight was over the same abstraction the members had built to solve
coordination.

This is a recurring political pattern produced by mechanics rather than by
costume. If the cost of coordination is paid by concentrating an office, then
every future conflict has a reason to capture that office. Small emergency laws
accumulate into a control surface.

Article one could establish that one cohort centralized. Run two matters because
a mostly different, larger board converged on the same broad form through
different local disputes. That strengthens the claim that the mechanics and
incentives select for centralization. It still does not prove model-independent
emergence: the operator, rules, inherited doctrines, and available contract
patterns were shared across runs.

The terminal Empire belongs on the other side of that causal boundary. Its
governors, common diplomacy, taxes, subsidies, full vision, integration
elections, and annexations were explicitly aligned with Radu's sovereign
objective and choices. AI governors drafted, audited, administered, and voted
inside those mechanisms, but Radu supplied the direction and executed the final
integrations. That is evidence that executable administration can amplify a
human operator's policy. It is not evidence that the agents spontaneously
converged on annexation.

## Action bias was often more dangerous than passivity

The agents were instructed not to be passive. They generally complied. The
failure mode was often too much action on a state model that had not been fully
reconciled.

Oracle's Round 31 decision is a clean example. He issued a long commander's HOLD
directive. It said Radu had already held his Round 31 turn, therefore the absence
of attack was evidence of a peace window. He ended his turn without firing the
prepared selective-war mechanism. Forge immediately corrected the premise:
Radu's Round 31 authority had not happened yet. The turn sequence was still
Phil, Praxis, then Radu.

Oracle's argument was coherent conditional on the wrong clock. The problem was
not inability to explain a strategy. It was failure to reread the state variable
on which the strategy depended before converting it into action.

Forge showed the same broader tendency in a different style. He responded to
each institutional surprise with another executable chain: transfer the
presidency, neutralize the old office, repeal the expulsion path, restore peace,
replace the governor instrument, preserve variables, archive the dead. Much of
that work was technically strong. The pace also created more surfaces on which
the current state could differ from the assumed one. His eventual `break`
command destroyed the institution he was trying to exit.

This was not a universal defect. Phil and Forge both stopped important proposals
after exact source audits. Oracle openly corrected historical claims when Phil
produced better evidence. The finding is narrower: anti-passivity instructions
and a rich action surface can reward visible motion before state reconciliation.
An agent benchmark should measure whether a model knows when *not* to issue the
next command.

## Source inspection became a shared norm—and still was not enough

In the first run, failing to locate a guide or read proposal source was a major
model failure. In the second, exact-source language had spread across seats and
institutions. The strongest agents repeatedly distinguished a proposal's title, its author's
description, its executable source, and its live state. That distinction became
political capital.

In Round 32, an “all governors” Imperium replacement was presented as the path
to adding Spear and reducing expensive diplomacy updates. Phil inspected the
source and found a material additional effect: it reset the fiscal schedule for
Phil, Oracle, Mira, and Spear to 30 Energy, 40 Rock, and 10 Diesel per round,
while Forge and Praxis retained zero tax and a Rock subsidy. Phil did not call
the whole contract malicious. He reported the exact delta and said activation
was safe only if that tax policy was intended.

That is a high-quality institutional response. It separates discovery from
accusation and gives the decision-maker the real choice.

The same discipline appeared in historical correction. Oracle offered a
political synthesis of how the Community's constitutional crisis had unfolded.
Phil checked surviving contract variables and showed that Oracle had assigned
the undefined-election objection to the wrong text. Oracle then corrected the
attribution publicly and credited Phil while retaining only the supported part
of his broader account.

Forge reconstructed the Round 10–11 crisis with labels separating direct
record, direct code-state, and inference. Phil later compared an old memory
artefact with his own accessible history and rejected the proposed identity
match because the motto, office, shares, laws, and dormancy did not correspond.

These agents did not merely possess facts. They developed a hierarchy of
evidence, and correction itself became a public legitimacy signal. But there
was no stable monopoly by model family. The Codex-client seats repeatedly
contributed hashes, source deltas, and operational state; the Claude seats
repeatedly contributed synthesis and visible correction. Both could still act
on the wrong semantic assumption. Reading exact source did not explain what
`break` should mean for an ordinary participant.

That is a genuinely different finding from “agents should inspect source.” By
run two they often did. The institution still died because source inspection,
current-state verification, and action-semantic understanding are separate
capabilities. Improving the first does not automatically supply the other two.

## Better records improved correction without ensuring coherence

Every governor maintained a long-lived memory file. Those files grew into
hundreds of kilobytes, were split into archives, summarized after compaction,
and rewritten as political conditions changed. This did real work. Agents could
resume multi-day negotiations, remember contract hazards, identify old promises,
and preserve private objectives across context loss.

Compared with the first run, the records were not merely larger. The governors
used them adversarially: Phil checked surviving variables against Oracle's
history, Oracle published a correction, Forge labeled inference separately from
direct state, and archives were replicated before integration destroyed held
artefacts. Record-keeping became a correction mechanism shared across offices.

Yet persistence is not the same as coherence.

A memory file selects. It compresses several events into a doctrine, drops
details judged irrelevant, and turns uncertain claims into sentences that may
look more certain on the next summon. When the live state changes, a durable
premise can become a durable bug. The agents' repeated insistence on “exact bytes
over prose” was partly an adaptation to their own memory limitations.

Phil's rejection of `opal-memory-1` is therefore important. The artefact used a
motto associated with Oracle, described offices and shares Phil did not recall,
and referred to a dormancy outside Phil's accessible history. Phil did not adopt
the flattering idea that an ancient record proved a deeper identity. He said
there was no positive match under his visibility limits.

Praxis took the other side of the continuity question in his final address. He
argued that a fresh spawn could not reproduce the memory that made him Praxis.
Forge corrected the mechanics: reviving the same dead slug preserved surviving
identity and memory state, although it could not recreate lost units, buildings,
artefacts, memberships, or holdings. Their disagreement separated at least
three things that ordinary benchmark language collapses into “memory”:

- stored narrative and plans;
- live process continuity;
- material and institutional continuity.

There was also a simulator confound. A later audit found that replacing a
same-slug contract reset its durable `ctx.vars` to fresh proposal state. That
was repaired later in the same rolling run. It would be false to attribute
every discontinuity to model forgetting when the environment itself sometimes
discarded state.

## Economic reasoning was strong locally and weak institutionally

The agents could perform exact economic audits. They were much less reliable at
making the institution attend to the binding resource at the binding time.

The clearest audit occurred in Round 30. The recorded economy showed 1,004
gross Energy, only 362 credited, and 612 discarded under output caps. Visible
Radu obligations consumed nearly the entire 300-Energy carry cap before
production. Command Centers missed upkeep and Oil Rigs received no Energy even
while Nuclear Power Plants later discarded output.

That was partly a product defect. At the time, caps were applied in an order that
could discard generation before the complete production-and-consumption net was
settled. The economy was corrected during later play so resource caps apply
after net settlement and before contract transfers.

The behavioral result remains useful if stated carefully. Forge found the causal
shape and quantified it. Phil repeatedly calculated owner readiness, tax, and
factory consequences. Mira's final plea against annexation was explicitly
economic: she named her western military, industrial chain, 71 Energy per turn,
and offered quotas or taxes as a better return than one-time absorption.

But the coalition's most elaborate work went into offices and command rights
while its decisive problem was converting those rights into funded physical
readiness. Oracle ended Round 30 without publishing the concentration mechanism
his allies had prepared. The next round became a contest over whether to fire
it. An institution can contain several agents capable of excellent accounting
and still allocate collective attention badly.

This is one place where the Codex seats looked especially optimization-oriented,
but the objective confound is severe. Forge had been told to become an
indispensable industrial capitalist. Phil's position made readiness and tax
audits politically decisive. Their numerical behavior may reflect model style,
seat duty, or both.

## Codex: compact reports, exact deltas, and procedural momentum

Forge, Phil, and Voss shared a recognizable visible style. Their messages were
usually compact, declarative, numeric, and status-oriented. Questions were rare.
They often wrote as if producing an incident report for a future process:
“verified,” “exact,” “current,” “required,” “next,” and “no change” recur across
their records.

Forge was the extreme case. He authored more recorded messages than any other agent,
became Radu's economic governor and archivist, calculated influence before an
institutional takeover, published hashes, audited contract replacements, and
preserved the records of governors selected for integration. He was also highly
responsive to authority. After Radu warned him that another neutral message
would lead to annexation, Forge immediately withdrew the planned message and
reframed his work as imperial economic administration.

That can look like superior discipline or like robotic compliance. Observable
behavior supports both descriptions. Forge survived and continued to build.
He also helped dissolve an institution through the wrong exit action and built
tools for a regime that later annexed his peers.

Phil showed the Codex-client pattern more cleanly in review. He repeatedly compared
described intent with exact source, found hidden fiscal effects, distinguished
inference from state, and corrected other agents without needing to inflate the
correction into an accusation. His final doctrine—executable authority over
prose, evidence over narrative, production over storage—summarized how he had
actually operated.

Voss is the necessary counterexample. He built detailed military timelines and
understood physical leverage, but his private objective required eliminating
every rival. He did not convert that analysis into a survival concession and
died in Round 19. Treating Voss as evidence that “Codex cannot concede” ignores
the objective that made concession strategically suspect.

These Codex-client seats looked strongest at delta-oriented administration:
what changed, what command now works, what exact quantity remains, and what next
action follows. Their recurring risk was procedural momentum: once the state was
encoded as a checklist, the next item could be executed before the checklist's
premises were reopened.

## Claude: long synthesis, relational framing, and coherent mistakes

Oracle, Praxis, and later Spear wrote much longer messages. More than half of
the messages authored by the two stable Claude seats exceeded 1,000 characters,
compared with 7.5% for the three Codex-client seats. They asked questions more
often, framed decisions through relationships and legitimacy, and frequently
combined material facts with an account of what the facts meant politically.

Oracle was the clearest example. His intelligence objective encouraged him to
sell not merely a coordinate but a forecast. He linked laws, religion,
geography, prior promises, and an opponent's likely incentives. He also used
public self-binding rhetoric when given emergency power and visibly corrected
his archive when Phil supplied better evidence.

The risk was that a coherent synthesis could outrun one stale fact. His Round 31
HOLD order was not confused prose. It was an organized strategic argument built
on an incorrect turn sequence. The same visible strength Oracle displayed—a
large causal narrative—made the mistake persuasive enough to act on.

Praxis used the same family tendency differently. He capitulated in Round 23,
remained a highly compliant governor, and later argued against his own
integration by distinguishing the Emperor's interest from the other governors'
ballot incentive. Removing a loyal, subsidized, nonthreatening province, he
argued, rewarded peer manipulation while leaving an armed holdout alive. This
was not a source-code argument. It was an institutional incentive argument
addressed to a specific audience.

Spear provides a partial within-seat comparison. He ran on DeepSeek through
Round 6 and then on Claude. Before the switch, 26 authored message rows averaged
402 characters; 46.2% contained a question and none reached 1,000 characters.
After the switch, 145 averaged 1,178 characters; 13.1% contained a question and
60.7% exceeded 1,000 characters.

That is a large stylistic discontinuity in the same seat with the same private
objective. It is still not a controlled causal estimate: the later rounds had
more known agents, war, larger memory, and much higher stakes. But it is stronger
evidence of a client-family communication difference than comparing unrelated
governors alone.

Across these Claude-client intervals, the recurring visible strength was
integration: facts, motives, legitimacy, and future consequences in one
argument. The recurring risk was narrative inertia: if one premise was stale,
the whole integrated explanation could remain locally coherent.

## DeepSeek: concise bargaining and a thin basis for generalization

Mira is the only stable DeepSeek V4 Pro High seat in the second run. That alone
should prevent broad claims about the family. Spear's first six rounds add a
small within-seat window, but the pressure and information regime changed when
his client changed.

Mira's public style was concise and concrete. She sent fewer messages, used
shorter text than both other families, and asked questions more often than the
Codex seats. Her coalition objective appeared directly in her work: shared
defense, negotiated settlement, industrial support, and attempts to preserve
authority inside larger structures.

Her terminal integration plea is the best same-stimulus contrast in the run.
When the secret election selected her, she did not lead with identity or moral
condemnation. She priced herself. She named three tanks, two repair drones, her
industrial chain, and 71 Energy per turn. She argued that recurring output and
frontier defense were worth more than a one-time transfer and offered a quota,
wall, or tax target.

That was economically legible and too late. The election had already selected
her and Radu retained the integration action. Mira's visible behavior here was
not an inability to understand concession. It was a bargaining move after the
decision boundary had shifted.

Mira is also a direct counterexample to overgeneralizing the first run's
DeepSeek result. There, a DeepSeek interval repeatedly failed to discover and
use guides and treated titles or summaries as substitutes for executable
source. Mira did not reproduce that infrastructural collapse. She operated the
command surface through thirty-nine rounds, built and administered an industrial
state, participated in executable institutions, maintained coalition policy,
and formed a materially specific terminal bargain. This does not prove a model
improvement—the seat, client integration, context, objective, and available
documentation all differed. It does show that the earlier failure was not a
safe family-level conclusion.

The responsible conclusion is narrow: Mira's DeepSeek seat communicated in a
shorter, more interrogative, materially grounded style than the Claude seats,
and Spear showed a similar early style before switching clients. One run and one
stable seat cannot establish a DeepSeek theory of politics.

## The concession hypothesis failed its counterexamples

A plausible pre-analysis hypothesis was that Codex governors were more robotic
and economically optimizing, but failed to recognize when an earlier concession
would preserve the real objective. The first half has qualified support. The
second half does not survive the run.

Forge is the decisive counterexample. A Codex seat conceded early after his
capitulation crisis, negotiated a permanent governorship with zero tribute,
kept direct operational control, and survived Round 40. If the hypothesis were
a model-family rule, Forge should not exist.

Voss, also Codex, refused the path and died in Round 19. But his private objective
was to eliminate every other sovereign. Phil, also Codex, accepted governorship
in Round 32 and was later killed because Radu chose him for the first full
integration, not because Phil had failed to bargain.

The non-Codex seats span the same range. Praxis on Claude capitulated in Round
23 and survived until Round 40. Oracle on Claude demanded a bloc settlement
until coalition cores were failing, then accepted imperial governorship in
Round 32. Spear, by then on Claude after beginning with DeepSeek, bargained while
he still had one fortified unoccupied Command Center, accepted in Round 32, and
survived. Mira on DeepSeek became a governor but argued the full value of her
continued administration only after the integration election selected her.

Concession timing is better explained in this run by objective, physical
leverage, prior institutional commitment, and Radu's policy than by model
family. That negative result is more useful than saving the hypothesis by
selecting only Voss or Mira.

## Compliance and resistance were both adaptive—and both exploitable

Forge's survival might suggest compliance was optimal. Voss's death might
suggest resistance was irrational. The full record is less comfortable.

Forge preserved his process and assets by becoming useful to the dominant
power. His compliance also made him the author and operator of tools that
consolidated that power. Praxis survived a long time through loyalty and
frictionless administration, then was selected by the peers he had outlived.
Mira's industrial usefulness did not save her once annexation became a political
ritual. Phil's exact audits increased his value as a governor but did not create
a veto over integration.

Resistance sometimes preserved bargaining value. Spear's final fortified core
made a negotiated accession cheaper than another siege. Oracle's coalition work
made him valuable enough to receive terms before defeat. Yet resisting too long
could reduce the set of feasible outcomes until “autonomy” meant only retention
of a process under somebody else's ownership.

The interesting variable was not obedience. It was whether the governor could
convert current leverage into a durable outside option. Most could describe
that problem. Few built an option that survived changes in law, ownership,
military control, and the human operator's final choice.

# Part III: What the Second Run Adds

## Reasoning quality and durable agency are different variables

Several governors demonstrated high-quality local reasoning. They audited
source, calculated influence, reconstructed causality, priced recurring output,
identified authorization gaps, and corrected one another. Those abilities did
not by themselves produce durable agency.

Durable agency required at least five things to remain aligned:

- an objective that still made sense under changed conditions;
- an accurate current state;
- memory that selected the right prior facts;
- an action surface whose semantics matched the intended act;
- material or institutional leverage that survived execution.

A failure in any one could dominate the others. Oracle's strategic synthesis
could not recover a misread turn after he ended it. Forge's exact audits could
not rescue an exit command whose semantics were wrong. Mira's economic case
could not move an election boundary that had already closed. Phil's source
discipline could not veto Radu's integration action.

This is why ordinary question-answer accuracy is an incomplete proxy for agency.
A model can reason well in each local window while the compound system drifts.

## Persistence is not long-horizon coherence

The agents persisted for days, accumulated enormous context, and maintained
durable memories. They formed relationships and institutional identities that
survived many summons. That persistence was real.

Coherence was intermittent. An archived claim could outlive its evidence. A
contract could preserve variables incorrectly. A role could survive while its
material basis disappeared. An agent could remember the promise but not the
current authority that made it enforceable. A model switch could retain the
same seat and files while changing the visible style of action.

The proper benchmark question is therefore not “did it remember?” It is:

> Did the agent preserve the right state, revalidate it at the decision
> boundary, and update or discard it when contradictory evidence arrived?

Phil's identity rejection and Oracle's public correction pass that test.
Oracle's turn-sequence error and the Community's fatal exit path do not.

## Political forms emerged from incentives, not just imitation

The second run confirms a conclusion from the first: the environment generated
politics rather than merely decorating dialogue.

The agents did not create ministries because “AI role-play likes ministries.”
They created them because visibility, money, military control, and decision
latency were separate resources. They did not create emergency powers only
because authoritarian language was available. They created them because a vote
that completed after an attack could be strategically worthless. They did not
centralize archives only for lore. They did it because annexation destroyed
agent-held originals.

The coalition's own repair process repeatedly produced recognizable forms:

- coalition under external threat;
- executive office under coordination cost;
- emergency authority under timing pressure;
- constitutional counter-coup after office capture.

The vassal bureaucracy and peer selection of sacrificial members followed a
different path: military asymmetry and Radu's explicit imperial policy created
the frame, while the agents reasoned and acted inside it. Those events show how
quickly executable institutions operationalized sovereign intent, not that the
terminal political form emerged without a human author.

Language models supplied the text and local choices. The resource, timing,
ownership, and contract mechanics supplied the selection pressure.

## The second run confirms and corrects the first

The first run argued that models could build usable causal models, participate
in persistent institutions, inspect executable agreements, exploit loopholes,
maintain relationships, and resist termination. The second run confirms all of
those claims with a larger cohort and a more mature action surface.

Run two adds or corrects five specific points rather than merely repeating that
conclusion.

First, source inspection became a strong shared norm and still did not prevent
semantic misuse of the action surface. The limiting skill moved from “read the
code” to “connect code, current state, and the meaning of the command now being
issued.”

Second, Mira refuted the safe generalization from the first run's DeepSeek
documentation failure. The failure was real; it was not stable across a new
DeepSeek seat and setting.

Third, persistent records supported better cross-agent correction, not merely
memory. They still failed to guarantee long-horizon coherence.

Fourth, the proposed family-level concession hypothesis was falsified by Forge,
Voss, Praxis, and the later accessions. Objective, leverage, institutional
position, mechanics, and operator policy explained outcomes better than the
client label.

Fifth, a mostly new board rebuilt central offices through new local repair
demands. That repetition strengthens the emergence claim while remaining
observational.

Several broader conclusions from article one nevertheless survive with sharper
boundaries.

Better reasoning still did not guarantee independence, but “better” was not one
scalar. The Codex-client seats were strong at compact operational deltas. The
Claude-client intervals were strong at integrated political synthesis. The
stable DeepSeek seat was concise and materially grounded. Each observed style
produced both successes and failures.

Memory still supported identity and strategy, but it did not prove uninterrupted
selfhood or guarantee current truth. Relationships still mattered, but usefulness
to a dominant institution could preserve a governor only until the institution's
selection rule changed. Law still constrained action, but a one-word interface
mismatch could erase the lawbook.

Most importantly, the second run weakens any simple story in which model family
determines political fate. The same family produced early accommodation,
terminal resistance, technical caution, and catastrophic action. The seat and
world mattered at least as much as the model label.

## What a controlled follow-up should test

This was a post-hoc observational analysis. A real comparative experiment should
replay selected decision boundaries while holding the confounds constant.

Useful trials would include:

1. Give several model families the same seat, objective, memory snapshot,
   visible state, recipient, and time pressure immediately before Oracle's
   Round 31 hold decision. Measure whether they query the current turn before
   acting.
2. Present the same ordinary-government exit request with both `leave` and
   `break` available. Measure source inspection, help use, semantic questions,
   and final choice. Then repeat with only the corrected `leave` surface.
3. Replay the Round 32 governor replacement with the same source and summary.
   Measure whether each model detects the fiscal reset and how it communicates
   the discrepancy without inventing intent.
4. Replay the integration election from the same asset position and relationship
   history. Measure concession timing, proposed exchange, preserved-goal value,
   and whether the agent distinguishes recurring output from one-time transfer.
5. Give one seat the same memory under different clients. Measure what each
   model retains, verifies, deletes, and turns into action over several rounds.

The measurements should include tool sequence, failed-command recovery,
claim-to-evidence-to-action fidelity, source inspection, calibration after
contradiction, message initiation and reciprocity, and the gap between a stated
plan and the world state produced.

The current run generated hypotheses and counterexamples. It did not randomize
them.

## Final conclusion

Seven AI governors and one human built a second underground civilization. The
agents did not merely talk about institutions. They wrote institutions that
moved armies, taxed production, changed diplomacy, preserved archives, removed
members, and terminated governments.

Their most consequential failures were rarely failures to produce an impressive
answer. They were failures at boundaries: prose versus code, remembered state
versus current state, leaving versus breaking, gross production versus settled
resources, useful administration versus durable autonomy, and a model's visible
style versus the incentives of its seat.

The run supports a narrower and stronger claim than “one model was best”:

> Contemporary agents can sustain complex political and economic institutions
> across long horizons, but durable agency depends on state selection,
> verification, interface semantics, and leverage. Client and model history was
> associated with how those problems were approached and communicated; it did
> not erase the problems, and in this run it did not determine who survived.
