package com.example.controller;

import com.example.model.User;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/users")
public class UserController {

    private final Map<String, User> userStorage = new HashMap<>();

    @PostMapping("/register")
    public ResponseEntity<String> registerUser(@RequestParam("username") String username) {
        if (userStorage.containsKey(username)) {
            return ResponseEntity.status(HttpStatus.CONFLICT).body("User already exists");
        }

        userStorage.put(username, new User(username));
        return ResponseEntity.status(HttpStatus.CREATED).body("User registered successfully");
    }

    @GetMapping("/{username}")
    public ResponseEntity<User> getUser(@PathVariable String username) {
        User user = userStorage.get(username);
        if (user == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body(null);
        }
        return ResponseEntity.ok(user);
    }

    @GetMapping("/getAll")
    public ResponseEntity<Map<String, User>> getAllUser() {
        if (userStorage.isEmpty()) {
            return ResponseEntity.status(HttpStatus.NO_CONTENT).body(null);
        }
        return ResponseEntity.ok(userStorage);
    }
}
