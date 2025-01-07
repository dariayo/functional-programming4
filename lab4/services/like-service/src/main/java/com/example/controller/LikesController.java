package com.example.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.client.RestTemplate;
import lombok.extern.slf4j.Slf4j;

import java.util.*;

@Slf4j
@RestController
@RequestMapping("/likes")
public class LikesController {
    private final Map<String, Set<String>> likes = new HashMap<>();

    @Autowired
    private RestTemplate restTemplate;

    private final String userServiceUrl = "http://user-service:8084/users/";
    private final String messageServiceUrl = "http://message-service:8081/messages/";

    @PostMapping("/{messageId}")
    public ResponseEntity<String> likeMessage(@PathVariable String messageId,
            @RequestParam("username") String username) {
        log.info("Received like request for messageId: {} from user: {}", messageId, username);

        // Проверка пользователя
        ResponseEntity<String> userResponse = restTemplate.getForEntity(userServiceUrl + username, String.class);
        if (!userResponse.getStatusCode().is2xxSuccessful()) {
            log.warn("User not found: {}", username);
            return ResponseEntity.status(404).body("User not found");
        }

        // Проверка сообщения
        ResponseEntity<String> messageResponse = restTemplate.getForEntity(messageServiceUrl + messageId, String.class);
        if (!messageResponse.getStatusCode().is2xxSuccessful()) {
            log.warn("Message not found: {}", messageId);
            return ResponseEntity.status(404).body("Message not found");
        }

        // Обработка лайка
        likes.putIfAbsent(messageId, new HashSet<>());
        likes.get(messageId).add(username);
        log.info("User {} liked messageId: {}", username, messageId);

        return ResponseEntity.ok("Liked successfully");
    }

    @GetMapping("/{messageId}")
    public ResponseEntity<Set<String>> getLikes(@PathVariable String messageId) {
        log.info("Fetching likes for messageId: {}", messageId);

        Set<String> userLikes = likes.getOrDefault(messageId, Collections.emptySet());
        log.info("Likes for messageId {}: {}", messageId, userLikes);

        return ResponseEntity.ok(userLikes);
    }
}
